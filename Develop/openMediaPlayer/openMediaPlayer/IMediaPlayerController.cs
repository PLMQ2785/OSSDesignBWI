using LibVLCSharp.Shared;
using openMediaPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface IMediaPlayerController : IDisposable
    {
        MediaPlayer MediaPlayer { get; }
        PlaybackState CurrentState { get; }
        bool IsPlaying { get; }
        long CurrentTime { get; }
        long Duration { get; }
        float Position { get; }
        string? CurrentMediaPath { get; }

        event EventHandler<string>? MediaOpened; //미디어 파일 경로 or null 전달
        event EventHandler<PlaybackState>? PlaybackStateChanged; //재생 상태 전달
        event EventHandler? TimeChanged; //재생 시간 변경시
        event EventHandler? LengthChanged; //전체 미디어 길이 변경시
        event EventHandler<string>? ErrorOccurred; //오류 메시지 전달
        event EventHandler? EndReached; //재생 종료시

        bool OpenMedia(string filePath);
        void Play();
        void Pause();
        void Stop();
        void Seek(float position);
        bool AddSubtitle(string subtitlePath);
    }


}
