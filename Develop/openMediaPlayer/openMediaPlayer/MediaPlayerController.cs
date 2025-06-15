using LibVLCSharp.Shared;
using openMediaPlayer.Models;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace openMediaPlayer.Services
{
    public class MediaPlayerController : IMediaPlayerController
    {
        private readonly LibVLC _libVLCEngine;
        private Media? _currentMediaVLC;
        private string? _currentMediaPathInternal;

        public MediaPlayer MediaPlayer { get; }

        //event Handler
        public event EventHandler<string>? MediaOpened;
        public event EventHandler<PlaybackState>? PlaybackStateChanged;
        public event EventHandler? TimeChanged;
        public event EventHandler? LengthChanged;
        public event EventHandler<string>? ErrorOccurred;
        public event EventHandler? EndReached; //재생 종료시

        //기본 재생 상태
        private PlaybackState _currentState = PlaybackState.Stopped;

        public PlaybackState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    PlaybackStateChanged?.Invoke(this, _currentState);
                }
            }
        }

        public bool IsPlaying => MediaPlayer.IsPlaying;
        public long CurrentTime => MediaPlayer.Time;
        public long Duration => MediaPlayer.Length;
        public float Position => MediaPlayer.Position;
        public string? CurrentMediaPath => _currentMediaPathInternal;
        public int Volume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = value;
        }

        public float PlaybackRate
        {
            get => MediaPlayer.Rate;
            set => MediaPlayer.SetRate(value); // LibVLC는 SetRate() 메서드를 사용합니다.
        }

        public MediaPlayerController()
        {
            try
            {
                Core.Initialize();
            }
            catch (Exception ex)
            {
                //초기화 실패 시 여기서 처리
                //System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            _libVLCEngine = new LibVLC();
            MediaPlayer = new MediaPlayer(_libVLCEngine);
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            MediaPlayer.Opening += (s, e) => CurrentState = PlaybackState.Opening;
            MediaPlayer.Playing += (s, e) => CurrentState = PlaybackState.Playing;
            MediaPlayer.Paused += (s, e) => CurrentState = PlaybackState.Paused;
            MediaPlayer.Stopped += (s, e) => CurrentState = PlaybackState.Stopped;
            MediaPlayer.EncounteredError += (s, e) =>
            {
                CurrentState = PlaybackState.Error;
                ErrorOccurred?.Invoke(this, "오류가 발생하였습니다.");
            };
            MediaPlayer.PositionChanged += (s, e) => TimeChanged?.Invoke(this, EventArgs.Empty);
            MediaPlayer.LengthChanged += (s, e) => LengthChanged?.Invoke(this, EventArgs.Empty);
            MediaPlayer.EndReached += (s, e) =>
            {
                CurrentState = PlaybackState.Stopped; // 상태를 Stopped로 변경
                EndReached?.Invoke(this, EventArgs.Empty); // EndReached 이벤트 발생
            };
        }

        public bool OpenMedia(string filePath, long? startTime = null)
        {
            try
            {
                MediaPlayer.Stop();
                MediaPlayer.Media = null; // 현재 미디어를 해제
                _currentMediaVLC?.Dispose();

                var mediaURI = new Uri(filePath);
                //_currentMediaVLC = new Media(_libVLCEngine, mediaURI); //":no-video-title-show <- 비디오 제목 숨기기, 필요시 추가

                //<추가1>
                if (startTime.HasValue && startTime.Value > 0)
                {
                    // :start-time은 초 단위이므로 밀리초를 1000으로 나눔
                    _currentMediaVLC = new Media(_libVLCEngine, mediaURI, $":start-time={startTime.Value / 1000}");
                }
                else
                {
                    _currentMediaVLC = new Media(_libVLCEngine, mediaURI);
                }
                //</추가1>

                MediaPlayer.Media = _currentMediaVLC;

                _currentMediaPathInternal = filePath;
                MediaOpened?.Invoke(this, filePath);
                CurrentState = PlaybackState.Stopped;

                return true;
            }
            catch (Exception ex)
            {
                //파일 열기 오류
                ErrorOccurred?.Invoke(this, $"미디어 파일을 여는 중 오류가 발생했습니다:\n {ex.Message}");
                _currentMediaPathInternal = null;
                MediaOpened?.Invoke(this, null);
                CurrentState = PlaybackState.Error;

                return false;
            }
        }

        public void Play()
        {
            if (MediaPlayer.Media != null)
            {
                MediaPlayer.Play();
            }
        }

        public void Pause()
        {
            if (MediaPlayer.CanPause)
            {
                MediaPlayer.Pause(); //Toggle
            }
        }

        public void Stop()
        {
            MediaPlayer.Stop();
            System.Threading.Thread.Sleep(100); // 잠시 대기 -> 미디어 플레이어가 정지 상태로 전환될 시간을 줌
        }

        public void Seek(float position)
        {
            if (MediaPlayer.IsSeekable && MediaPlayer.Length > 0)
            {
                
                long newTime = (long)(MediaPlayer.Length * Math.Clamp(position, 0f, 1f));

                
                MediaPlayer.Time = newTime;
            }
        }

        public bool AddSubtitle(string subtitlePath)
        {
            if (MediaPlayer.Media != null && File.Exists(subtitlePath))
            {
                var subtitleFileURI = new Uri(subtitlePath).AbsoluteUri;
                bool success = MediaPlayer.AddSlave(MediaSlaveType.Subtitle, subtitleFileURI, true);
                if (!success)
                {
                    ErrorOccurred?.Invoke(this, "자막 불러오기 실패");
                }

                //미디어 재생 중 자막 표시되게 수정
                int currentSPU = MediaPlayer.Spu; //자막 트랙 인덱스
                if (currentSPU > 0) //valid Track Selected
                {
                    MediaPlayer.SetSpu(0);//자막 off
                    MediaPlayer.SetSpu(currentSPU); //자막 다시 선택
                }

                return success;
            }
            ErrorOccurred?.Invoke(this, "자막 파일이 존재하지 않거나 미디어가 열려있지 않습니다.");
            return false;
        }

        public void Dispose()
        {
            MediaPlayer?.Stop();
            MediaPlayer?.Dispose();
            _currentMediaVLC?.Dispose();
            _libVLCEngine?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void SeekRelative(int seconds)
        {
            // 탐색이 불가능한 미디어이거나, 미디어가 없으면 아무것도 하지 않음
            if (!MediaPlayer.IsSeekable || MediaPlayer.Length <= 0)
            {
                System.Diagnostics.Debug.WriteLine("Seek failed: Media is not seekable or duration is not yet known.");
                return;
            }

            // 현재 시간(밀리초)에 원하는 시간(초 * 1000)을 더함
            long newTime = MediaPlayer.Time + (seconds * 1000);

            // 계산된 시간이 0보다 작거나 전체 길이보다 크지 않도록 범위를 제한
            newTime = Math.Clamp(newTime, 0, MediaPlayer.Length);

            // 최종 계산된 시간으로 이동
            MediaPlayer.Time = newTime;
        }
    }
}
