using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Models
{
    public class SubtitleSettings
    {
        public SubtitleDisplaySettings Display { get; set; } = new SubtitleDisplaySettings();
        public SubtitleLoadSyncSettings LoadSync { get; set; } = new SubtitleLoadSyncSettings();
        public SubtitleSTTSettings STT { get; set; } = new SubtitleSTTSettings();
    }

    public enum SubtitlePosition
    {
        Bottom,
        Top
    }

    public class SubtitleDisplaySettings
    {
        public string DefaultFont { get; set; } = "Arial";
        public double SizeScale { get; set; } = 1.0;
        public string FontColor { get; set; } = "#FFFFFFFF"; // White
        public string OutlineColor { get; set; } = "#FF000000"; // Black
        public SubtitlePosition Position { get; set; } = SubtitlePosition.Bottom;
    }

    public class SubtitleLoadSyncSettings
    {
        public string DefaultEncoding { get; set; } = "UTF-8";
        public string PreferredLanguages { get; set; } = "KOR,ENG"; // 쉼표로 구분
        public int DefaultSyncMilliseconds { get; set; } = 0;
    }

    public class SubtitleSTTSettings
    {
        public string DefaultSTTModel { get; set; } = "base";
        public string DefaultSTTLanguage { get; set; } = "ko";
        public bool AutoGenerateOnOpen { get; set; } = false;
    }
}
