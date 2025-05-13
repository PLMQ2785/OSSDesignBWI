using LibVLCSharp.Shared;
using openMediaPlayer.Models;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        private string? _currentMediaFilePathVM;
        private bool _IsUserDraggingSlider = false;

        public MediaPlayer VLCPlayerEngine => _mediaPlayerController.MediaPlayer;

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

        public RelayCommandController OpenCommand { get; }
        public RelayCommandController PlayCommand { get; }
        public RelayCommandController PauseCommand { get; }
        public RelayCommandController StopCommand { get; }
        public RelayCommandController GenerateSubtitlesCommand { get; }

        public MainViewModel(IMediaPlayerController mediaPlayerController, IMediaFileController mediaFileController,
                             ISubtitleController subtitleController, ITimeFormatter timeFormatter, IDispatcherController dispatcherController,
                             IPreferencesController preferencesController)
        {
            _mediaPlayerController = mediaPlayerController ?? throw new ArgumentNullException(nameof(mediaPlayerController));
            _mediaFileController = mediaFileController ?? throw new ArgumentNullException(nameof(mediaFileController));
            _subtitleController = subtitleController ?? throw new ArgumentNullException(nameof(subtitleController));
            _timeFormatter = timeFormatter ?? throw new ArgumentNullException(nameof(timeFormatter));
            _dispatcherController = dispatcherController ?? throw new ArgumentNullException(nameof(dispatcherController));
            _preferencesController = preferencesController ?? throw new ArgumentNullException(nameof(preferencesController));

            OpenCommand = new RelayCommandController(async param => await OpenMediaAsync());
            PlayCommand = new RelayCommandController(param => _mediaPlayerController.Play(), param => IsMediaLoaded && !_mediaPlayerController.IsPlaying);
            PauseCommand = new RelayCommandController(param => _mediaPlayerController.Pause(), param => IsMediaLoaded && _mediaPlayerController.IsPlaying);
            StopCommand = new RelayCommandController(param => _mediaPlayerController.Stop(), param => IsMediaLoaded);
            GenerateSubtitlesCommand = new RelayCommandController(async param => await GenerateSubtitlesAsync(), param => CanGenerateSubtitles);

            SubscribeToServiceEvents();

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

            _subtitleController.SubtitleGenerationStarted += OnSubtitleGenerationStarted;
            _subtitleController.SubtitleGenerationCompleted += OnSubtitleGenerationCompleted;
        }

        private async Task OpenMediaAsync()
        {
            var filePath = _mediaFileController.SelectMediaFile();
            if (!string.IsNullOrEmpty(filePath))
            {
                StatusMessage = "파일 여는 중...";
                IsMediaLoaded = false; // 로딩 중으로 표시
                UpdateCommandCanExecute();

                bool opened = _mediaPlayerController.OpenMedia(filePath);
                // 실제 상태 업데이트는 OnMediaPlayerMediaOpened 이벤트 핸들러에서 처리
            }
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
            });
        }
    }
}
