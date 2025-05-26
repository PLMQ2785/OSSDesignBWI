using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System;
using Microsoft.Win32;
using LibVLCSharp.Shared;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

// <내가 구현한 인터페이스들>
using openMediaPlayer.ViewModels;
using openMediaPlayer.Services;
using openMediaPlayer.Services.Interfaces; // 인터페이스 네임스페이스
// </내가 구현한 인터페이스들>

namespace openMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 프로토타입 구현 코드
    //public partial class MainWindow : Window
    //{
    //    private LibVLC _libVLC;
    //    private MediaPlayer _mediaPlayer;
    //    private bool _isUserDraggingSlider = false;

    //    private string _currentMediaFilePath = null;

    //    private string _ffmpegPathExecutable = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "bin", "ffmpeg.exe");
    //    private string _whisperPathExecutable = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "whisper", "whisper-cli.exe");
    //    private string _whisperModelPathOnly = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "whisper", "models");

    //    //기본코드
    //    public MainWindow()
    //    {
    //        InitializeComponent();

    //        //Init
    //        _libVLC = new LibVLC();
    //        _mediaPlayer = new MediaPlayer(_libVLC);

    //        //Attach MediaPlayer to the VideoView
    //        videoView.MediaPlayer = _mediaPlayer;

    //        _mediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
    //        _mediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
    //        _mediaPlayer.Playing += (s, e) => Dispatcher.Invoke(() =>
    //        {
    //            UpdateControlsEnabledState(true);
    //            GenerateSubtitlesButton.IsEnabled = !string.IsNullOrEmpty(_currentMediaFilePath); // 파일 로드 시에만 Activate
    //        });
    //        _mediaPlayer.Paused += (s, e) => Dispatcher.Invoke(() => UpdateControlsEnabledState(true)); // 일시정지 시에도 컨트롤 활성화 유지
    //        _mediaPlayer.Stopped += (s, e) => Dispatcher.Invoke(() =>
    //        {
    //            UpdateControlsEnabledState(false);
    //            CurrentTimeLabel.Text = FormatTime(0); // 정지 시 현재 시간 00:00
    //            PositionSlider.Value = 0;
    //            // TotalDurationLabel은 그대로 유지하거나, 새 파일 열 때 초기화

    //            GenerateSubtitlesButton.IsEnabled = false;
    //        });
    //        _mediaPlayer.EncounteredError += (s, e) => Dispatcher.Invoke(() => UpdateControlsEnabledState(false));
    //    }
    //    private void OpenFileButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        var openFileDialog = new OpenFileDialog();

    //        if (openFileDialog.ShowDialog() == true)
    //        {
    //            try
    //            {
    //                string filePath = openFileDialog.FileName;

    //                _currentMediaFilePath = filePath; //파일 경로 저장

    //                //clear prev media
    //                _mediaPlayer.Stop();
    //                _mediaPlayer.Media?.Dispose();

    //                //new media object
    //                var media = new Media(_libVLC, new Uri(filePath));
    //                _mediaPlayer.Media = media;
    //                media.Dispose();

    //                //test auto play
    //                //_mediaPlayer.Play();

    //                GenerateSubtitlesButton.IsEnabled = !string.IsNullOrEmpty(_currentMediaFilePath); // 파일 로드 시에만 Activate
    //            }
    //            catch (Exception ex)
    //            {
    //                MessageBox.Show($"파일을 열거나 재생하는 중 오류가 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);

    //                _currentMediaFilePath = null;
    //                GenerateSubtitlesButton.IsEnabled = false;
    //                UpdateControlsEnabledState(false); //disable controls
    //            }
    //        }
    //    }

    //    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    //    {
    //        // Dispose of the MediaPlayer and LibVLC instances
    //        _mediaPlayer?.Stop();
    //        _mediaPlayer?.Dispose();
    //        _libVLC.Dispose();
    //    }

    //    private void PlayButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        if (_mediaPlayer != null)
    //        {
    //            _mediaPlayer.Play();
    //        }
    //        else
    //        {
    //            MessageBox.Show("미디어 파일을 먼저 불러와 주세요", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
    //        }
    //    }

    //    private void PauseButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        _mediaPlayer?.Pause();
    //    }

    //    private void StopButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        _mediaPlayer?.Stop();
    //    }

    //    private void PositionSlider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //    {
    //        if (PositionSlider.IsEnabled) _isUserDraggingSlider = true;
    //    }

    //    private void PositionSlider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    //    {
    //        if (_isUserDraggingSlider && PositionSlider.IsEnabled)
    //        {
    //            _mediaPlayer.Position = (float)PositionSlider.Value; //set media position
    //        }
    //        _isUserDraggingSlider = false;
    //    }

    //    private string FormatTime(long timeInMilliseconds)
    //    {
    //        if (timeInMilliseconds < 0) { timeInMilliseconds = 0; }
    //        TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeInMilliseconds);
    //        if (timeSpan.TotalHours >= 1)
    //        {
    //            return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
    //        }
    //        else
    //        {
    //            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    //        }
    //    }

    //    private void UpdateControlsEnabledState(bool isMediaActive)
    //    {
    //        PositionSlider.IsEnabled = isMediaActive;
    //    }

    //    private void MediaPlayer_LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
    //    {
    //        Dispatcher.Invoke(() =>
    //        {
    //            TotalDurationLabel.Text = FormatTime(e.Length); //display time update
    //            UpdateControlsEnabledState(e.Length > 0); //enable/disable controls
    //        });
    //    }

    //    private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
    //    {
    //        Dispatcher.Invoke(() =>
    //        {
    //            CurrentTimeLabel.Text = FormatTime(_mediaPlayer.Time); //current play time
    //            if (!_isUserDraggingSlider) { PositionSlider.Value = e.Position; }

    //        });
    //    }

    //    private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    //    {

    //    }

    //    private async void GenerateSubtitlesButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        // Check if a media file is loaded
    //        if (string.IsNullOrEmpty(_currentMediaFilePath))
    //        {
    //            MessageBox.Show("먼저 미디어 파일을 열어주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
    //            return;
    //        }

    //        if (!File.Exists(_whisperPathExecutable))
    //        {
    //            MessageBox.Show("Whisper 실행 파일이 존재하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
    //            return;
    //        }

    //        if (!File.Exists(Path.Combine(_whisperModelPathOnly, "base.bin")))
    //        {
    //            MessageBox.Show("whisper 모델이 존재하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
    //            return;
    //        }

    //        GenerateSubtitlesButton.IsEnabled = false;
    //        this.Title = "자막 생성중...";

    //        try
    //        {
    //            //1. 오디오 추출
    //            string tempAudioFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.wav");
    //            bool isAudioExtracted = await ExtractAudioAsync(_currentMediaFilePath, tempAudioFilePath);

    //            if (!isAudioExtracted)
    //            {
    //                MessageBox.Show("오디오 추출에 실패했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
    //                return;
    //            }

    //            //2. 자막 생성
    //            string srtFilePath = Path.ChangeExtension(tempAudioFilePath, ".srt"); //임시 오디오 파일 이름으로 만듬
    //            bool isSubtitleGenerated = await GenerateSubtitleAsync(tempAudioFilePath, srtFilePath);

    //            if (!isSubtitleGenerated)
    //            {
    //                MessageBox.Show("자막 생성에 실패했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
    //                return;
    //            }

    //            //3. 자막을 미디어 플레이어에 로드
    //            var srtFileURI = new Uri(srtFilePath).AbsoluteUri;
    //            _mediaPlayer.AddSlave(MediaSlaveType.Subtitle, srtFileURI, true); //true: 자막 즉시 선택

    //            //미디어 재생 중 자막 표시되게 수정
    //            int currentSPU = _mediaPlayer.Spu; //자막 트랙 인덱스
    //            if (currentSPU > 0) //valid Track Selected
    //            {
    //                await Dispatcher.InvokeAsync(() =>
    //                {
    //                    _mediaPlayer.SetSpu(0);//자막 off
    //                    _mediaPlayer.SetSpu(currentSPU); //자막 다시 선택
    //                });
    //            }
    //            MessageBox.Show("자막을 불러왔습니다!", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show($"자막 생성 중 오류 발생:{ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
    //        }
    //        finally
    //        {
    //            GenerateSubtitlesButton.IsEnabled = (!string.IsNullOrEmpty(_currentMediaFilePath) && _mediaPlayer.IsPlaying); //파일 로드 시에만 Activate
    //            this.Title = "Open Media Player";
    //        }
    //    }

    //    private Task<bool> ExtractAudioAsync(string videoFilePath, string outputAudioFilePath)
    //    {
    //        return Task.Run(() =>
    //        {
    //            string arguments = $"-i \"{videoFilePath}\" -y -vn -acodec pcm_s16le \"{outputAudioFilePath}\"";

    //            ProcessStartInfo startInfo = new ProcessStartInfo
    //            {
    //                FileName = _ffmpegPathExecutable,
    //                Arguments = arguments,
    //                UseShellExecute = false,
    //                CreateNoWindow = true,
    //                RedirectStandardError = true,
    //                RedirectStandardOutput = true
    //            };

    //            using (Process process = Process.Start(startInfo))
    //            {
    //                //표준 출력/오류 스트림 읽기
    //                string stdErr = process.StandardError.ReadToEnd();
    //                string stdOut = process.StandardOutput.ReadToEnd();
    //                process.WaitForExit();

    //                //성공여부 판단
    //                if (process.ExitCode != 0)
    //                {
    //                    //Console.WriteLine($"FFMPEG ER:{stdErr}");//for debug
    //                    //세부 오류 메시지 추가 시 여기에
    //                    return false;
    //                }

    //                //오디오 추출 성공
    //                return File.Exists(outputAudioFilePath);
    //            }
    //        });
    //    }

    //    private Task<bool> GenerateSubtitleAsync(string audioFilePath, string outputSrtBaseFilePath, string whisperModelName = "")
    //    {
    //        //확장자 제외 기본파일 이름만 전달 필요할수있음.
    //        //whisper cpp main.exe에 -osrt 옵션 사용 시 .str붙여서 만듬.
    //        //srtFilePath가 .srt포함 전체 경로로 사용될것임.
    //        string outputDirectory = Path.GetDirectoryName(outputSrtBaseFilePath);
    //        string outputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputSrtBaseFilePath);

    //        return Task.Run(() =>
    //        {
    //            if (string.IsNullOrEmpty(whisperModelName))
    //            {
    //                whisperModelName = "base.bin"; //default
    //            }
    //            string modelPath = Path.Combine(_whisperModelPathOnly, whisperModelName);



    //            string arguments = $"-m \"{modelPath}\" -f \"{audioFilePath}\" -l auto -osrt -of \"{Path.Combine(outputDirectory, outputFileNameWithoutExtension)}\"";

    //            ProcessStartInfo startInfo = new ProcessStartInfo
    //            {
    //                FileName = _whisperPathExecutable,
    //                Arguments = arguments,
    //                UseShellExecute = false,
    //                CreateNoWindow = true,
    //                RedirectStandardError = true,
    //                RedirectStandardOutput = true,
    //                WorkingDirectory = Path.GetDirectoryName(_whisperPathExecutable) ?? "" //일부 경우 특정 작업 디렉토리 요구할 수도 있음.
    //            };

    //            using (Process process = Process.Start(startInfo))
    //            {
    //                //표준 출력/오류 스트림 읽기
    //                string stdErr = process.StandardError.ReadToEnd();
    //                string stdOut = process.StandardOutput.ReadToEnd();

    //                //Console.WriteLine($"Whisper stdOut:{stdOut}");//for debug
    //                //Console.WriteLine($"Whisper stdErr:{stdErr}");//for debug
    //                process.WaitForExit();

    //                //성공여부 판단
    //                if (process.ExitCode != 0)
    //                {
    //                    //예외처리시 아래 주석 사용
    //                    //throw new Exception($"Whisper.cpp failed with exit code {process.ExitCode}.\nError: {stderr}");

    //                    MessageBox.Show($"Whisper.cpp 오류 발생: {stdErr}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
    //                    //세부 오류 메시지 추가 시 여기에
    //                    return false;
    //                }
    //                //자막 생성 성공, 최종 생성 파일명 확인
    //                return File.Exists(outputSrtBaseFilePath);
    //            }

    //        });
    //    }


    //}

    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly IMediaPlayerController _mediaPlayerController; // Dispose를 위해 인터페이스 타입으로 참조

        public MainWindow()
        {
            InitializeComponent();

            // 서비스 인스턴스 생성
            // 실제 애플리케이션에서는 DI 컨테이너(예: Microsoft.Extensions.DependencyInjection) 사용을 강력히 권장합니다.
            IDispatcherController dispatcherService = new DispatcherController();
            IPreferencesController configService = new PreferenceController();
            ITimeFormatter timeFormatter = new TimeFormatter();
            IProcessRunner processRunner = new ProcessRunner();
            IAudioExtractor audioExtractor = new AudioExtractor(configService, processRunner);
            ISubtitleGenerator subtitleGenerator = new SubtitleGenerator(configService, processRunner);
            // IMediaPlayerService를 구현한 MediaPlayerService 인스턴스 생성
            _mediaPlayerController = new MediaPlayerController();
            IMediaFileController mediaFileService = new MediaFileController();
            ISubtitleController subtitleService = new SubtitleController(audioExtractor, subtitleGenerator, _mediaPlayerController, configService);

            IPlaylistController playlistService = new PlaylistController(_mediaPlayerController); // PlaylistController 생성 및 주입

            // 뷰모델 인스턴스 생성 및 DataContext 설정
            _viewModel = new MainViewModel(
                _mediaPlayerController,
                mediaFileService,
                subtitleService,
                timeFormatter,
                dispatcherService,
                configService,
                playlistService);
            DataContext = _viewModel;

            // VideoView에 MediaPlayer 할당
            // _viewModel.VlcMediaPlayer는 _mediaPlayerService.MediaPlayer를 반환합니다.
            videoView.MediaPlayer = _viewModel.VLCPlayerEngine;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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



    }
}