using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using Microsoft.Win32;
using LibVLCSharp.Shared;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace openMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private bool _isUserDraggingSlider = false;

        //기본코드
        public MainWindow()
        {
            InitializeComponent();

            //Init
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            //Attach MediaPlayer to the VideoView
            videoView.MediaPlayer = _mediaPlayer;

            _mediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
            _mediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
            _mediaPlayer.Playing += (s, e) => Dispatcher.Invoke(() => UpdateControlsEnabledState(true));
            _mediaPlayer.Paused += (s, e) => Dispatcher.Invoke(() => UpdateControlsEnabledState(true)); // 일시정지 시에도 컨트롤 활성화 유지
            _mediaPlayer.Stopped += (s, e) => Dispatcher.Invoke(() =>
            {
                UpdateControlsEnabledState(false);
                CurrentTimeLabel.Text = FormatTime(0); // 정지 시 현재 시간 00:00
                PositionSlider.Value = 0;
                // TotalDurationLabel은 그대로 유지하거나, 새 파일 열 때 초기화
            });
            _mediaPlayer.EncounteredError += (s, e) => Dispatcher.Invoke(() => UpdateControlsEnabledState(false));
        }
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;

                    //clear prev media
                    _mediaPlayer.Stop();
                    _mediaPlayer.Media?.Dispose();

                    //new media object
                    var media = new Media(_libVLC, new Uri(filePath));
                    _mediaPlayer.Media = media;
                    media.Dispose();

                    //test auto play
                    _mediaPlayer.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일을 열거나 재생하는 중 오류가 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateControlsEnabledState(false); //disable controls
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Dispose of the MediaPlayer and LibVLC instances
            _mediaPlayer?.Stop();
            _mediaPlayer?.Dispose();
            _libVLC.Dispose();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Play();
            }
            else
            {
                MessageBox.Show("미디어 파일을 먼저 불러와 주세요", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer?.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer?.Stop();
        }

        private void PositionSlider_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PositionSlider.IsEnabled) _isUserDraggingSlider = true;
        }

        private void PositionSlider_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isUserDraggingSlider && PositionSlider.IsEnabled)
            {
                _mediaPlayer.Position = (float)PositionSlider.Value; //set media position
            }
            _isUserDraggingSlider = false;
        }

        private string FormatTime(long timeInMilliseconds)
        {
            if (timeInMilliseconds < 0) { timeInMilliseconds = 0; }
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeInMilliseconds);
            if (timeSpan.TotalHours >= 1)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            }
        }

        private void UpdateControlsEnabledState(bool isMediaActive)
        {
            PositionSlider.IsEnabled = isMediaActive;
        }

        private void MediaPlayer_LengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                TotalDurationLabel.Text = FormatTime(e.Length); //display time update
                UpdateControlsEnabledState(e.Length > 0); //enable/disable controls
            });
        }

        private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CurrentTimeLabel.Text = FormatTime(_mediaPlayer.Time); //current play time
                if (!_isUserDraggingSlider) { PositionSlider.Value = e.Position; }

            });
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}