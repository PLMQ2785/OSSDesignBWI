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

        public bool OpenMedia(string filePath)
        {
            try
            {
                MediaPlayer.Stop();
                _currentMediaVLC?.Dispose();

                var mediaURI = new Uri(filePath);
                _currentMediaVLC = new Media(_libVLCEngine, mediaURI); //":no-video-title-show <- 비디오 제목 숨기기, 필요시 추가
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
        }

        public void Seek(float position)
        {
            if (MediaPlayer.IsSeekable && MediaPlayer.Media != null)
            {
                MediaPlayer.Position = Math.Clamp(position, 0f, 1f);
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
    }
}
