using openMediaPlayer.Models;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class PlaylistController : IPlaylistController
    {
        private readonly IMediaPlayerController _mediaPlayerController;
        private readonly ObservableCollection<MediaItem> _playlist = new ObservableCollection<MediaItem>();
        private int _currentTrackIndex = -1;

        public event EventHandler? PlaylistUpdated;
        public event EventHandler<MediaItem?>? CurrentTrackChanged;

        public IEnumerable<MediaItem> CurrentPlaylist => _playlist;
        public MediaItem? CurrentTrack => (_currentTrackIndex >= 0 && _currentTrackIndex < _playlist.Count)
                                          ? _playlist[_currentTrackIndex]
                                          : null;

        private bool _isRepeatEnabled = false;
        public bool IsRepeatEnabled
        {
            get => _isRepeatEnabled;
            set => _isRepeatEnabled = value;
        }

        public PlaylistController(IMediaPlayerController mediaPlayerController)
        {
            _mediaPlayerController = mediaPlayerController;
            _mediaPlayerController.EndReached += OnMediaPlayerEndReached; // 재생 종료 이벤트 구독
            _playlist.CollectionChanged += (s, e) => PlaylistUpdated?.Invoke(this, EventArgs.Empty); // 컬렉션 변경 시 이벤트 발생
        }

        private void OnMediaPlayerEndReached(object? sender, EventArgs e)
        {
            // 재생이 끝나면 자동으로 다음 트랙 재생
            NextTrack();
        }

        public void AddMedia(string filePath)
        {
            try
            {
                var newItem = new MediaItem(filePath);
                if (!_playlist.Contains(newItem))
                {
                    _playlist.Add(newItem);
                }
            }
            catch (ArgumentException) { /* 파일 경로 오류 처리 */ }
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
                _playlist.RemoveAt(index);
                if (index == _currentTrackIndex)
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
            int index = _playlist.IndexOf(item);
            if (index != -1)
            {
                _currentTrackIndex = index;
                if (_mediaPlayerController.OpenMedia(item.FilePath))
                {
                    _mediaPlayerController.Play();
                    CurrentTrackChanged?.Invoke(this, item);
                }
                else
                {
                    // 파일 열기 실패 시 다음 트랙 시도 (또는 오류 처리)
                    NextTrack();
                }
            }
        }

        public void NextTrack()
        {
            if (_playlist.Count == 0) return;

            int nextIndex = _currentTrackIndex + 1;

            if (nextIndex >= _playlist.Count)
            {
                if (IsRepeatEnabled)
                {
                    nextIndex = 0; // 반복 활성화 시 처음으로
                }
                else
                {
                    // 반복 비활성화 시 재생 중지
                    _mediaPlayerController.Stop();
                    _currentTrackIndex = -1; // 현재 트랙 없음으로 표시
                    CurrentTrackChanged?.Invoke(this, null);
                    return;
                }
            }

            PlayTrack(_playlist[nextIndex]);
        }

        public void PreviousTrack()
        {
            if (_playlist.Count == 0) return;

            int prevIndex = _currentTrackIndex - 1;

            if (prevIndex < 0)
            {
                if (IsRepeatEnabled)
                {
                    prevIndex = _playlist.Count - 1; // 반복 활성화 시 마지막으로
                }
                else
                {
                    prevIndex = 0; // 반복 비활성화 시 처음으로 (또는 정지)
                }
            }

            PlayTrack(_playlist[prevIndex]);
        }
    }
}
