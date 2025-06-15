using LibVLCSharp.Shared;
using openMediaPlayer.Models;
using openMediaPlayer.Services;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

//된다 눈물난다 ㅠㅠㅠㅠㅠ
namespace openMediaPlayer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IMediaPlayerController _mediaPlayerController;
        private readonly IMediaFileController _mediaFileController;
        private readonly ISubtitleController _subtitleController;
        private readonly ITimeFormatter _timeFormatter;
        private readonly IDispatcherController _dispatcherController;
        private readonly IPreferencesController _preferencesController;
        private readonly IPlaylistController _playlistController;
        private readonly ISettingsController _settingsController; //추가
        private readonly ILiveSupportController _liveSupportController; // 추가

        private string? _currentMediaFilePathVM;
        private bool _IsUserDraggingSlider = false;

        public MediaPlayer VLCPlayerEngine => _mediaPlayerController.MediaPlayer;
        public AppSettings Settings => _settingsController.CurrentSettings;

        private bool _isLlmBusy;
        public bool IsLlmBusy
        {
            get => _isLlmBusy;
            private set
            {
                if (SetProperty(ref _isLlmBusy, value))
                {
                    OnPropertyChanged(nameof(IsLlmInputEnabled));
                    SubmitLlmCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLlmInputEnabled => !IsLlmBusy;

        // <Live Support 관련 속성 및 명령 추가>
        private string _llmInputText = "";
        public string LlmInputText
        {
            get => _llmInputText;
            set => SetProperty(ref _llmInputText, value);
        }

        private string _llmHistory = "";
        public string LlmHistory
        {
            get => _llmHistory;
            set => SetProperty(ref _llmHistory, value);
        }
        public RelayCommandController SubmitLlmCommand { get; }
        // </Live Support 관련 속성 및 명령 추가>

        private string _CurrentTimeText = "00:00:00";
        public string CurrentTimeText
        {
            get => _CurrentTimeText;
            set => SetProperty(ref _CurrentTimeText, value);
        }

        private string _TotalDurationText = "00:00:00";
        public string TotalDurationText
        {
            get => _TotalDurationText;
            set => SetProperty(ref _TotalDurationText, value);
        }

        private double _SliderPosition;
        public double SliderPosition
        {
            get => _SliderPosition;
            set
            {
                if (SetProperty(ref _SliderPosition, value) && _IsUserDraggingSlider)
                {

                    CurrentTimeText = _timeFormatter.FormatTime((long)(_SliderPosition * _mediaPlayerController.Duration));

                }
            }
        }

        public double SliderMaxValue => 1.0; //LibVLC에서 Position이 0-1

        private bool _IsMediaLoaded;
        public bool IsMediaLoaded
        {
            get => _IsMediaLoaded;
            set
            {
                if (SetProperty(ref _IsMediaLoaded, value))
                {
                    // IsMediaLoaded가 변경되면 CanGenerateSubtitles도 업데이트
                    OnPropertyChanged(nameof(CanGenerateSubtitles));

                    //IsMediaLoaded가 true로 변경되면 OpenCommand, PlayCommand, PauseCommand, StopCommand의 CanExecute도 업데이트
                    UpdateCommandCanExecute();
                }
            }
        }

        // CanGenerateSubtitles는 IsMediaLoaded에 따라 달라지므로, IsMediaLoaded 변경 시 함께 OnPropertyChanged 호출
        public bool CanGenerateSubtitles => IsMediaLoaded && !IsGeneratingSubtitles;


        private bool _IsPlaying;
        public bool IsPlaying
        {
            get => _IsPlaying;
            set => SetProperty(ref _IsPlaying, value);
        }

        private string _WindowTitle = "Open Media Player";
        public string WindowTitle
        {
            get => _WindowTitle;
            set => SetProperty(ref _WindowTitle, value);
        }

        private string _StatusMessage = "준비";
        public string StatusMessage
        {
            get => _StatusMessage;
            set => SetProperty(ref _StatusMessage, value);
        }

        private bool _IsGeneratingSubtitles;
        public bool IsGeneratingSubtitles
        {
            get => _IsGeneratingSubtitles;
            set
            {
                if (SetProperty(ref _IsGeneratingSubtitles, value))
                {
                    // IsGeneratingSubtitles가 변경되면 CanGenerateSubtitles도 업데이트
                    OnPropertyChanged(nameof(CanGenerateSubtitles));

                    // IsGeneratingSubtitles가 true로 변경되면 GenerateSubtitlesCommand의 CanExecute도 업데이트
                    UpdateCommandCanExecute();
                }
            }
        }

        // 재생 목록 관련 속성들...
        private ObservableCollection<MediaItem> _playlistItems = new ObservableCollection<MediaItem>();
        public ObservableCollection<MediaItem> PlaylistItems
        {
            get => _playlistItems;
            set => SetProperty(ref _playlistItems, value);
        }

        private MediaItem? _selectedPlaylistItem;
        public MediaItem? SelectedPlaylistItem
        {
            get => _selectedPlaylistItem;
            set => SetProperty(ref _selectedPlaylistItem, value);
        }

        private MediaItem? _currentPlaylistItem;
        public MediaItem? CurrentPlaylistItem
        {
            get => _currentPlaylistItem;
            set => SetProperty(ref _currentPlaylistItem, value);
        }

        private int _lastVolumeBeforeMute = 100; //초기 볼륨..
        public int Volume
        {
            get => _mediaPlayerController.Volume;
            set
            {
                if (_mediaPlayerController.Volume != value)
                {
                    _mediaPlayerController.Volume = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMuted));
                }
            }
        }

        public bool IsMuted => Volume == 0;



        public RelayCommandController OpenCommand { get; }
        public RelayCommandController PlayCommand { get; }
        public RelayCommandController PauseCommand { get; }
        public RelayCommandController StopCommand { get; }
        public RelayCommandController GenerateSubtitlesCommand { get; }

        // 재생 목록 관련 명령들...
        public RelayCommandController AddFilesToPlaylistCommand { get; }
        public RelayCommandController PlaySelectedCommand { get; }
        public RelayCommandController RemoveSelectedCommand { get; }
        public RelayCommandController ClearPlaylistCommand { get; }
        public RelayCommandController NextTrackCommand { get; }
        public RelayCommandController PreviousTrackCommand { get; }
        public RelayCommandController OpenSettingsWindowCommand { get; } //추가

        public RelayCommandController MuteCommand { get; }
        public RelayCommandController VolumeUpCommand { get; } // 볼륨 증가
        public RelayCommandController VolumeDownCommand { get; } // 볼륨 감소
        public RelayCommandController SeekForwardCommand { get; }
        public RelayCommandController SeekBackwardCommand { get; }


        public MainViewModel(IMediaPlayerController mediaPlayerController, IMediaFileController mediaFileController, ISubtitleController subtitleController,
                                ITimeFormatter timeFormatter, IDispatcherController dispatcherController, IPreferencesController preferencesController,
                                IPlaylistController playlistController, ISettingsController settingsController, ILiveSupportController liveSupportController)
        {
            _mediaPlayerController = mediaPlayerController ?? throw new ArgumentNullException(nameof(mediaPlayerController));
            _mediaFileController = mediaFileController ?? throw new ArgumentNullException(nameof(mediaFileController));
            _subtitleController = subtitleController ?? throw new ArgumentNullException(nameof(subtitleController));
            _timeFormatter = timeFormatter ?? throw new ArgumentNullException(nameof(timeFormatter));
            _dispatcherController = dispatcherController ?? throw new ArgumentNullException(nameof(dispatcherController));
            _preferencesController = preferencesController ?? throw new ArgumentNullException(nameof(preferencesController));
            _playlistController = playlistController ?? throw new ArgumentNullException(nameof(playlistController));
            _settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController)); //추가
            _liveSupportController = liveSupportController ?? throw new ArgumentNullException(nameof(liveSupportController)); //추가

            OpenCommand = new RelayCommandController(async param => await OpenMediaAsync());
            PlayCommand = new RelayCommandController(param => _mediaPlayerController.Play(), param => IsMediaLoaded && !_mediaPlayerController.IsPlaying);
            PauseCommand = new RelayCommandController(param => _mediaPlayerController.Pause(), param => IsMediaLoaded && _mediaPlayerController.IsPlaying);
            StopCommand = new RelayCommandController(param => _mediaPlayerController.Stop(), param => IsMediaLoaded);
            GenerateSubtitlesCommand = new RelayCommandController(async param => await GenerateSubtitlesAsync(), param => CanGenerateSubtitles);
            // 설정 창 열기 명령 초기화
            OpenSettingsWindowCommand = new RelayCommandController(param => OpenSettingsWindow());

            MuteCommand = new RelayCommandController(param => ToggleMute());
            VolumeUpCommand = new RelayCommandController(param => Volume = Math.Min(Volume + 5, 100));
            VolumeDownCommand = new RelayCommandController(param => Volume = Math.Max(Volume - 5, 0));


            //볼륨 설정 외부에서 바뀔일이 있을까 싶긴 한데 일단 혹시몰라서..
            _mediaPlayerController.MediaPlayer.VolumeChanged += (s, e) => OnPropertyChanged(nameof(Volume));

            // 설정 변경 이벤트 구독
            _settingsController.SettingsChanged += OnSettingsChanged;

            // PlaylistController에 설정 반영
            _playlistController.IsRepeatEnabled = _settingsController.CurrentSettings.Playback.RepeatPlaylist;

            SubmitLlmCommand = new RelayCommandController(
                                                            async param => await ExecuteLlmCommand(),
                                                            param => !string.IsNullOrWhiteSpace(LlmInputText) && _liveSupportController.IsInitialized && !IsLlmBusy
                                                         );

            AddFilesToPlaylistCommand = new RelayCommandController(async param => await AddFilesToPlaylistAsync());
            PlaySelectedCommand = new RelayCommandController(param => PlaySelectedItem(), param => SelectedPlaylistItem != null);
            RemoveSelectedCommand = new RelayCommandController(param => RemoveSelectedItem(), param => SelectedPlaylistItem != null);
            ClearPlaylistCommand = new RelayCommandController(param => _playlistController.ClearPlaylist(), param => PlaylistItems.Any());
            NextTrackCommand = new RelayCommandController(param => _playlistController.NextTrack(), param => PlaylistItems.Any());
            PreviousTrackCommand = new RelayCommandController(param => _playlistController.PreviousTrack(), param => PlaylistItems.Any());

            var canSeek = new Predicate<object?>(param => IsMediaLoaded && _mediaPlayerController.MediaPlayer.IsSeekable);
            SeekForwardCommand = new RelayCommandController(param => SeekForward(), canSeek);
            SeekBackwardCommand = new RelayCommandController(param => SeekBackward(), canSeek);

            SubscribeToServiceEvents();

            UpdatePlaylistView();

            // 애플리케이션 시작 시 의존성 검사
            if (!_preferencesController.CheckDependencies(out string errorMessage))
            {
                StatusMessage = $"오류: {errorMessage}";
                // MessageBox.Show(errorMessage, "의존성 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SubscribeToServiceEvents()
        {
            _mediaPlayerController.MediaOpened += OnMediaPlayerMediaOpened;
            _mediaPlayerController.PlaybackStateChanged += OnMediaPlayerStateChanged;
            _mediaPlayerController.TimeChanged += OnMediaPlayerTimeChanged;
            _mediaPlayerController.LengthChanged += OnMediaPlayerLengthChanged;
            _mediaPlayerController.ErrorOccurred += OnMediaPlayerErrorOccurred;
            _mediaPlayerController.EndReached += OnMediaPlayerEndReached;

            _subtitleController.SubtitleGenerationStarted += OnSubtitleGenerationStarted;
            _subtitleController.SubtitleGenerationCompleted += OnSubtitleGenerationCompleted;

            _playlistController.PlaylistUpdated += OnPlaylistUpdated;
            _playlistController.CurrentTrackChanged += OnCurrentTrackChanged;
        }



        private async Task GenerateSubtitlesAsync()
        {
            if (string.IsNullOrEmpty(_currentMediaFilePathVM) || IsGeneratingSubtitles) return;
            await _subtitleController.GenerateAndLoadSubtitlesAsync(_currentMediaFilePathVM);
        }

        public void SetSliderDragging(bool isDragging)
        {
            _IsUserDraggingSlider = isDragging;
            if (!isDragging && IsMediaLoaded)
            {
                _mediaPlayerController.Seek((float)SliderPosition);
            }
        }

        private void OnMediaPlayerMediaOpened(object? sender, string? filePath)
        {
            _dispatcherController.Invoke(() =>
            {
                IsMediaLoaded = !string.IsNullOrEmpty(filePath);
                if (IsMediaLoaded)
                {
                    _currentMediaFilePathVM = filePath;
                    WindowTitle = Path.GetFileName(filePath);
                    StatusMessage = "미디어 로드 완료.";
                    // 새 파일 로드 시 UI 초기화
                    CurrentTimeText = _timeFormatter.FormatTime(0);
                    // LengthChanged 이벤트가 발생하므로 TotalDurationText는 거기서 업데이트
                    SliderPosition = 0;
                }
                else
                {
                    _currentMediaFilePathVM = null;
                    WindowTitle = "Open Media Player";
                    StatusMessage = "미디어를 열지 못했습니다.";
                    CurrentTimeText = _timeFormatter.FormatTime(0);
                    TotalDurationText = _timeFormatter.FormatTime(0);
                    SliderPosition = 0;
                }
                UpdateCommandCanExecute();
            });
        }

        private void OnMediaPlayerStateChanged(object? sender, PlaybackState state)
        {
            _dispatcherController.Invoke(() =>
            {
                IsPlaying = state == PlaybackState.Playing;
                switch (state)
                {
                    case PlaybackState.Playing: StatusMessage = "재생 중"; break;
                    case PlaybackState.Paused: StatusMessage = "일시 정지"; break;
                    case PlaybackState.Stopped:
                        StatusMessage = "정지됨";
                        CurrentTimeText = _timeFormatter.FormatTime(0);
                        if (!_IsUserDraggingSlider) SliderPosition = 0;
                        break;
                    case PlaybackState.Opening: StatusMessage = "여는 중..."; break;
                    case PlaybackState.Error: StatusMessage = "오류 발생"; break; // 상세 메시지는 ErrorOccurred에서
                }
                UpdateCommandCanExecute();
            });
        }

        private void OnMediaPlayerTimeChanged(object? sender, EventArgs e)
        {
            _dispatcherController.Invoke(() =>
            {
                if (!_IsUserDraggingSlider)
                {
                    CurrentTimeText = _timeFormatter.FormatTime(_mediaPlayerController.CurrentTime);
                    SliderPosition = _mediaPlayerController.Position;
                }
            });
        }

        private void OnMediaPlayerLengthChanged(object? sender, EventArgs e)
        {
            _dispatcherController.Invoke(() =>
            {
                TotalDurationText = _timeFormatter.FormatTime(_mediaPlayerController.Duration);
            });
        }

        private void OnMediaPlayerErrorOccurred(object? sender, string errorMessage)
        {
            _dispatcherController.Invoke(() =>
            {
                StatusMessage = $"플레이어 오류: {errorMessage}";
                // MessageBox.Show(errorMessage, "플레이어 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void OnSubtitleGenerationStarted(object? sender, EventArgs e)
        {
            _dispatcherController.Invoke(() =>
            {
                IsGeneratingSubtitles = true;
                StatusMessage = "자막 생성 중... 시간이 걸릴 수 있습니다.";
                WindowTitle = "자막 생성 중... - " + Path.GetFileName(_currentMediaFilePathVM);
                UpdateCommandCanExecute();
            });
        }

        private void OnSubtitleGenerationCompleted(object? sender, (bool success, string messageOrPath) result)
        {
            _dispatcherController.Invoke(() =>
            {
                IsGeneratingSubtitles = false;
                StatusMessage = result.messageOrPath;
                if (_currentMediaFilePathVM != null)
                {
                    WindowTitle = Path.GetFileName(_currentMediaFilePathVM);
                }
                else
                {
                    WindowTitle = "Open Media Player";
                }

                if (result.success)
                {
                    // MessageBox.Show("자막이 성공적으로 로드되었습니다.", "자막 로드 성공", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(result.messageOrPath, "자막 생성 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                UpdateCommandCanExecute();
            });
        }

        private void UpdateCommandCanExecute()
        {
            _dispatcherController.Invoke(() =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                PlayCommand.RaiseCanExecuteChanged();
                PauseCommand.RaiseCanExecuteChanged();
                StopCommand.RaiseCanExecuteChanged();
                GenerateSubtitlesCommand.RaiseCanExecuteChanged();

                AddFilesToPlaylistCommand.RaiseCanExecuteChanged();
                PlaySelectedCommand.RaiseCanExecuteChanged();
                RemoveSelectedCommand.RaiseCanExecuteChanged();
                ClearPlaylistCommand.RaiseCanExecuteChanged();
                NextTrackCommand.RaiseCanExecuteChanged();
                PreviousTrackCommand.RaiseCanExecuteChanged();
            });
        }

        private void OnMediaPlayerEndReached(object? sender, EventArgs e)
        {
            // PlaylistController가 이미 EndReached를 처리하므로,
            // MainViewModel에서는 UI 업데이트 등 추가 작업이 필요하면 여기에 작성합니다.
            // 여기서는 PlaylistController가 다음 트랙을 재생하도록 둡니다.
        }

        private void OnPlaylistUpdated(object? sender, EventArgs e)
        {
            _dispatcherController.Invoke(UpdatePlaylistView);
        }

        private void OnCurrentTrackChanged(object? sender, MediaItem? currentItem)
        {
            _dispatcherController.Invoke(() =>
            {
                CurrentPlaylistItem = currentItem;
                if (currentItem != null)
                {
                    // 현재 재생 중인 미디어가 변경되었으므로,
                    // 미디어 로드 관련 UI 업데이트 로직을 호출할 수 있습니다.
                    // (OnMediaPlayerMediaOpened와 유사한 로직 필요 시)
                    IsMediaLoaded = true;
                    WindowTitle = Path.GetFileName(currentItem.FilePath);
                    StatusMessage = "재생 중";
                }
                else if (_mediaPlayerController.CurrentState == PlaybackState.Stopped)
                {
                    IsMediaLoaded = false;
                    WindowTitle = "Open Media Player";
                    StatusMessage = "정지됨";
                }

                var previousTrack = PlaylistItems.FirstOrDefault(item => item.IsPlaying);
                if (previousTrack != null)
                {
                    previousTrack.IsPlaying = false;
                }

                // 새 트랙의 IsPlaying = true
                if (currentItem != null)
                {
                    var currentVmItem = PlaylistItems.FirstOrDefault(item => item.Equals(currentItem));
                    if (currentVmItem != null)
                    {
                        currentVmItem.IsPlaying = true;
                    }
                }
                CurrentPlaylistItem = currentItem; // ViewModel의 CurrentPlaylistItem도 업데이트


                UpdateCommandCanExecute();
            });
        }


        private void UpdatePlaylistView()
        {
            PlaylistItems.Clear();
            foreach (var item in _playlistController.CurrentPlaylist)
            {
                PlaylistItems.Add(item);
            }
            UpdateCommandCanExecute(); // 재생 목록 변경 시 명령 상태 업데이트
        }


        //private async Task OpenMediaAsync()
        //{
        //    var filePath = _mediaFileController.SelectMediaFile();
        //    if (!string.IsNullOrEmpty(filePath))
        //    {
        //        StatusMessage = "파일 여는 중...";
        //        IsMediaLoaded = false; // 로딩 중으로 표시
        //        UpdateCommandCanExecute();

        //        bool opened = _mediaPlayerController.OpenMedia(filePath);
        //        // 실제 상태 업데이트는 OnMediaPlayerMediaOpened 이벤트 핸들러에서 처리
        //    }
        //}

        private async Task OpenMediaAsync()
        {
            if (_mediaPlayerController.IsPlaying)
            {
                _mediaPlayerController.Stop();
            }

            var filePath = _mediaFileController.SelectMediaFile();
            if (!string.IsNullOrEmpty(filePath))
            {
                StatusMessage = "파일 여는 중...";
                IsMediaLoaded = false;
                UpdateCommandCanExecute();

                // 파일을 열 때, 기존 재생 목록을 지우고 새로 추가 후 재생
                _playlistController.ClearPlaylist();
                _playlistController.AddMedia(filePath);
                var firstItem = _playlistController.CurrentPlaylist.FirstOrDefault();
                if (firstItem != null)
                {
                    _playlistController.PlayTrack(firstItem);
                }
            }
        }

        private async Task AddFilesToPlaylistAsync()
        {
            var filePaths = _mediaFileController.SelectMediaFiles();
            if (filePaths != null && filePaths.Any())
            {
                _playlistController.AddMultipleMedia(filePaths);
            }
        }

        private void PlaySelectedItem()
        {
            if (SelectedPlaylistItem != null)
            {
                _playlistController.PlayTrack(SelectedPlaylistItem);
            }
        }

        private void RemoveSelectedItem()
        {
            if (SelectedPlaylistItem != null)
            {
                _playlistController.RemoveMedia(SelectedPlaylistItem);
            }
        }

        // 설정 변경 시 호출
        private void OnSettingsChanged(object? sender, AppSettings newSettings)
        {
            _dispatcherController.Invoke(() =>
            {
                _playlistController.IsRepeatEnabled = newSettings.Playback.RepeatPlaylist;

                OnPropertyChanged(nameof(Settings));

                // 만약 미디어가 현재 재생 중이라면,
                if (_mediaPlayerController.IsPlaying)
                {
                    // 새로 저장된 재생 속도를 즉시 플레이어에 적용합니다.
                    _mediaPlayerController.PlaybackRate = newSettings.Playback.DefaultPlaybackSpeed;
                }
            });
        }

        // 설정 창 열기 요청 이벤트
        public event EventHandler? RequestOpenSettingsWindow;

        private void OpenSettingsWindow()
        {
            RequestOpenSettingsWindow?.Invoke(this, EventArgs.Empty);
        }

        private async Task ExecuteLlmCommand()
        {
            if (IsLlmBusy) return;

            string userInput = LlmInputText;
            LlmHistory += $"> User: {userInput}\n";
            LlmInputText = ""; // 입력창 비우기

            try
            {
                IsLlmBusy = true;
                string result = await _liveSupportController.ProcessUserInputAsync(userInput);
                LlmHistory += $"> Assistant: {result}\n";
            }
            catch (Exception ex)
            {
                LlmHistory += $"> System Error: {ex.Message}\n";
            }
            finally
            {
                IsLlmBusy = false;
            }
        }

        private void ToggleMute()
        {
            if (IsMuted)
            {
                Volume = _lastVolumeBeforeMute;
            }
            else
            {
                _lastVolumeBeforeMute = Volume;
                Volume = 0;
            }
        }
        private void SeekForward()
        {
            int interval = _settingsController.CurrentSettings.Playback.SeekIntervalSeconds;
            _mediaPlayerController.SeekRelative(interval);
        }

        private void SeekBackward()
        {
            int interval = _settingsController.CurrentSettings.Playback.SeekIntervalSeconds;
            _mediaPlayerController.SeekRelative(-interval); // 뒤로 가기는 음수 값 사용
        }
    }
}
