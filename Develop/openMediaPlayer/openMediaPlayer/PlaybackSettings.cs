using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Models
{
    public class PlaybackSettings
    {
        public float DefaultPlaybackSpeed { get; set; } = 1.0f; // 기본 재생 속도
        public int SeekIntervalSeconds { get; set; } = 10; // 탐색 간격 (초)
        public bool RepeatPlaylist { get; set; } = false; // 재생 목록 반복
        public bool AlwaysOnTop { get; set; } = false; // 항상 위에 표시
    }
}
