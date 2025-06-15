using LibVLCSharp.Shared;
using Microsoft.Win32;
using openMediaPlayer.Models;
using openMediaPlayer.Services;
using openMediaPlayer.Services.Interfaces; // 인터페이스 네임스페이스
// <내가 구현한 인터페이스들>
using openMediaPlayer.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
// </내가 구현한 인터페이스들>

namespace openMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly IMediaPlayerController _mediaPlayerController; // Dispose를 위해 인터페이스 타입으로 참조
        private readonly ISettingsController _settingsService; // 추가
        private readonly IPlayerActionRegistry _actionRegistry; // 추가
        private readonly ILiveSupportController _liveSupportService; // 추가

        public MainWindow()
        {
            InitializeComponent();

            // 서비스 인스턴스 생성
            IDispatcherController dispatcherService = new DispatcherController();
            IPreferencesController configService = new PreferenceController();
            ITimeFormatter timeFormatter = new TimeFormatter();
            IProcessRunner processRunner = new ProcessRunner();
            IAudioExtractor audioExtractor = new AudioExtractor(configService, processRunner);
            ISubtitleGenerator subtitleGenerator = new SubtitleGenerator(configService, processRunner);
            // IMediaPlayerService를 구현한 MediaPlayerService 인스턴스 생성
            _mediaPlayerController = new MediaPlayerController();
            IMediaFileController mediaFileService = new MediaFileController();

            //IPlaylistController playlistService = new PlaylistController(_mediaPlayerController); // PlaylistController 생성 및 주입


            // 설정 서비스 1.
            string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            _settingsService = new SettingsController(settingsFilePath);
            // 비동기 로드 (주의: 생성자에서 await 사용 불가하므로 Task.Run 등으로 처리하거나,
            // 별도 초기화 메서드에서 호출. 여기서는 간단하게 동기적 대기.)
            Task.Run(async () => await _settingsService.LoadSettingsAsync()).Wait();

            ISubtitleController subtitleService = new SubtitleController(audioExtractor, subtitleGenerator, _mediaPlayerController, configService, _settingsService);

            IPlaylistController playlistService = new PlaylistController(_mediaPlayerController, _settingsService, subtitleService, dispatcherService); // PlaylistController 생성 및 주입

            // PlayerActionRegistry 추가
            IPlayerActionRegistry actionRegistry = new PlayerActionRegistry(
                _mediaPlayerController,
                playlistService,
                subtitleService
            );
            _actionRegistry = actionRegistry; // 필드에 저장

            _liveSupportService = new LiveSupportController(configService, actionRegistry, dispatcherService);

            // 뷰모델 인스턴스 생성 및 DataContext 설정
            _viewModel = new MainViewModel(
                _mediaPlayerController,
                mediaFileService,
                subtitleService,
                timeFormatter,
                dispatcherService,
                configService,
                playlistService,
                _settingsService,
                _liveSupportService);
            DataContext = _viewModel;

            // VideoView에 MediaPlayer 할당
            // _viewModel.VlcMediaPlayer는 _mediaPlayerService.MediaPlayer를 반환.
            videoView.MediaPlayer = _viewModel.VLCPlayerEngine;

            _viewModel.RequestOpenSettingsWindow += OnRequestOpenSettingsWindow;

            // 창이 로드된 후 LLM 모델을 초기화
            this.Loaded += async (s, e) => await _liveSupportService.InitializeAsync();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 종료 시 재생 위치 저장, try 없이 하니까 에러생기면 멈춰버려서 try 넣음
            try
            {
                // 설정이 켜져 있고, 현재 재생 중인 미디어가 있을 때
                if (_settingsService.CurrentSettings.General.RememberLastPosition &&
                    !string.IsNullOrEmpty(_mediaPlayerController.CurrentMediaPath))
                {
                    string path = _mediaPlayerController.CurrentMediaPath;
                    long time = _mediaPlayerController.CurrentTime;
                    float position = _mediaPlayerController.Position;

                    // 재생이 거의 끝났으면(98% 이상) 저장하지 않고 기록에서 제거
                    if (position > 0.98f)
                    {
                        _settingsService.CurrentSettings.PlaybackPositions.Remove(path);
                    }
                    else
                    {
                        _settingsService.CurrentSettings.PlaybackPositions[path] = time;
                        //System.Diagnostics.Debug.WriteLine($"Saving position for {path} at {time} ms."); // <<<--- 디버그용
                    }
                    // 설정을 파일에 동기적으로 저장.
                    // Closing 이벤트에서는 비동기 대기가 불안정할 수 있으므로 GetAwaiter().GetResult() 사용.
                    //_settingsService.SaveSettingsAsync().GetAwaiter().GetResult(); -> 이거 쓰니까 데드락생기는지 뻗음
                    await _settingsService.SaveSettingsAsync(); // 비동기 저장
                }
            }
            catch (Exception ex)
            {
                // 오류 발생해도 프로그램 안터지게
                System.Diagnostics.Debug.WriteLine($"Failed to save settings on closing: {ex.Message}");
            }


            // IMediaPlayerService가 IDisposable을 구현하므로, Dispose 호출
            _mediaPlayerController?.Dispose();
        }

        private void PositionSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (PositionSlider.IsEnabled)
            {
                _viewModel.SetSliderDragging(true);
            }
        }

        // PositionSlider_ValueChanged 이벤트는 ViewModel의 SliderPosition 속성 변경으로 처리되므로
        // 여기서 특별히 처리할 필요는 없다. (TwoWay 바인딩 사용 시)
        // 사용자가 드래그할 때 특별 처리가 필요하면 PreviewMouse 이벤트들을 사용.
        private void PositionSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PositionSlider.IsEnabled)
            {
                _viewModel.SetSliderDragging(false); // ViewModel이 Seek 처리
            }
        }

        private void OnRequestOpenSettingsWindow(object? sender, EventArgs e)
        {
            // SettingsViewModel 생성 시 설정 서비스를 전달합니다.
            var settingsViewModel = new SettingsViewModel(_settingsService);
            var settingsWindow = new SettingsWindow(settingsViewModel)
            {
                Owner = this // 부모 창을 현재 창으로 설정 (중앙에 표시됨)
            };
            settingsWindow.ShowDialog(); // 다른 작업을 막는 모달 대화상자로 창을 엽니다.
        }


    }
}