using openMediaPlayer.Models; // AppSettings 사용을 위해 추가
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace openMediaPlayer.Services
{
    public class PlaylistController : IPlaylistController
    {
        private readonly IMediaPlayerController _mediaPlayerController;
        private readonly ISettingsController _settingsController;
        private readonly ISubtitleController _subtitleController; //추가
        private readonly IDispatcherController _dispatcherController; // 추가
        private readonly ObservableCollection<MediaItem> _playlist = new ObservableCollection<MediaItem>();

        private int _currentTrackIndex = -1;

        public event EventHandler? PlaylistUpdated;
        public event EventHandler<MediaItem?>? CurrentTrackChanged;

        public IEnumerable<MediaItem> CurrentPlaylist => _playlist;
        public MediaItem? CurrentTrack => (_currentTrackIndex >= 0 && _currentTrackIndex < _playlist.Count)
                                          ? _playlist[_currentTrackIndex]
                                          : null;

        public bool IsRepeatEnabled { get; set; }

        public PlaylistController(IMediaPlayerController mediaPlayerController, ISettingsController settingsController, ISubtitleController subtitleController, IDispatcherController dispatcherController)
        {
            _mediaPlayerController = mediaPlayerController ?? throw new ArgumentNullException(nameof(mediaPlayerController));
            _settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController)); // settingsController 할당
            _subtitleController = subtitleController ?? throw new ArgumentNullException(nameof(subtitleController)); // 추가
            _dispatcherController = dispatcherController ?? throw new ArgumentNullException(nameof(dispatcherController));

            // 1. 설정 변경 이벤트를 구독
            _settingsController.SettingsChanged += OnSettingsChanged;

            // 2. 컨트롤러 생성 시 초기 설정을 반영
            ApplySettings(_settingsController.CurrentSettings);

            _mediaPlayerController.EndReached += OnMediaPlayerEndReached;
            _playlist.CollectionChanged += (s, e) => PlaylistUpdated?.Invoke(this, EventArgs.Empty);
            _subtitleController = subtitleController;
        }

        // 설정이 변경될 때마다 호출
        private void OnSettingsChanged(object? sender, AppSettings newSettings)
        {
            ApplySettings(newSettings);
        }

        // 설정을 적용
        private void ApplySettings(AppSettings settings)
        {
            IsRepeatEnabled = settings.Playback.RepeatPlaylist;
        }

        private void OnMediaPlayerEndReached(object? sender, EventArgs e)
        {
            //NextTrack();
            _dispatcherController.InvokeAsync(() =>
            {
                NextTrack();
            });
        }

        public void AddMedia(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var newItem = new MediaItem(filePath);
                    if (!_playlist.Contains(newItem))
                    {
                        _playlist.Add(newItem);
                    }
                }
            }
            catch (ArgumentException) { /* 파일 경로 오류 처리, 안하면 뻗어서 추가 */ }
        }

        public void AddMultipleMedia(IEnumerable<string> filePaths)
        {
            foreach (var path in filePaths)
            {
                AddMedia(path);
            }
        }

        public void RemoveMedia(MediaItem item)
        {
            int index = _playlist.IndexOf(item);
            if (index != -1)
            {
                bool wasCurrent = index == _currentTrackIndex;
                _playlist.RemoveAt(index);

                if (wasCurrent)
                {
                    _mediaPlayerController.Stop();
                    _currentTrackIndex = -1;
                    CurrentTrackChanged?.Invoke(this, null);
                }
                else if (index < _currentTrackIndex)
                {
                    _currentTrackIndex--;
                }
            }
        }

        public void ClearPlaylist()
        {
            _mediaPlayerController.Stop();
            _playlist.Clear();
            _currentTrackIndex = -1;
            CurrentTrackChanged?.Invoke(this, null);
        }

        public void PlayTrack(MediaItem item)
        {
            _mediaPlayerController.Pause();
            System.Threading.Thread.Sleep(100); // Stop 처리 대기
            ////if (_mediaPlayerController.IsPlaying)
            ////{
            ////}


            int index = _playlist.IndexOf(item);
            if (index != -1)
            {
                _currentTrackIndex = index;

                //<추가1>
                long? startTime = null;
                if ((_settingsController.CurrentSettings.General.RememberLastPosition &&
                     _settingsController.CurrentSettings.PlaybackPositions.TryGetValue(item.FilePath, out long savedTime)))
                {
                    startTime = savedTime;
                }
                //</추가1>


                if (_mediaPlayerController.OpenMedia(item.FilePath, startTime))
                {
                    _mediaPlayerController.Play();
                    CurrentTrackChanged?.Invoke(this, item);

                    try
                    {
                        // 1. 설정에서 기본 재생 속도 값을 가져오고.
                        float playbackRate = _settingsController.CurrentSettings.Playback.DefaultPlaybackSpeed;

                        // 2. 플레이어에 해당 속도를 적용.
                        _mediaPlayerController.PlaybackRate = playbackRate;
                    }
                    catch (Exception ex)
                    {
                        // 설정 값 오류 등으로 실패할 경우를 대비
                        System.Diagnostics.Debug.WriteLine($"Failed to set playback rate: {ex.Message}");
                    }

                    //열때 자막생성 추가
                    //설정값 true인지 보고
                    if (_settingsController.CurrentSettings.Subtitles.STT.AutoGenerateOnOpen)
                    {
                        // STT 설정 가져오고.
                        string model = _settingsController.CurrentSettings.Subtitles.STT.DefaultSTTModel;
                        string lang = _settingsController.CurrentSettings.Subtitles.STT.DefaultSTTLanguage;

                        // 자막 생성을 백그라운드에서 시작
                        _ = _subtitleController.GenerateAndLoadSubtitlesAsync(item.FilePath, model); //Fire and Forget
                    }


                }
                else
                {
                    NextTrack();
                }
            }
        }

        public void NextTrack()
        {
            if (_playlist.Count == 0) return;

            _mediaPlayerController.Stop();

            int nextIndex = _currentTrackIndex + 1;

            if (nextIndex >= _playlist.Count)
            {
                if (IsRepeatEnabled)
                {
                    nextIndex = 0;
                }
                else
                {
                    _mediaPlayerController.Stop();
                    _currentTrackIndex = -1;
                    CurrentTrackChanged?.Invoke(this, null);
                    return;
                }
            }

            PlayTrack(_playlist[nextIndex]);
        }

        public void PreviousTrack()
        {
            if (_playlist.Count == 0) return;

            _mediaPlayerController.Stop();

            int prevIndex = _currentTrackIndex - 1;

            if (prevIndex < 0)
            {
                if (IsRepeatEnabled)
                {
                    prevIndex = _playlist.Count - 1;
                }
                else
                {
                    prevIndex = 0;
                }
            }

            PlayTrack(_playlist[prevIndex]);
        }
    }
}