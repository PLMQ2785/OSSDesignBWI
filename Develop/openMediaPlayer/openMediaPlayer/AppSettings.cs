using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using openMediaPlayer.Models;

namespace openMediaPlayer.Models
{
    public class AppSettings
    {
        public GeneralSettings General { get; set; } = new GeneralSettings();
        public PlaybackSettings Playback { get; set; } = new PlaybackSettings();
        public SubtitleSettings Subtitles { get; set; } = new SubtitleSettings();
        public LiveSupportSettings LiveSupport { get; set; } = new LiveSupportSettings();

        [JsonInclude] // private set을 사용해도 JSON에 포함되게
        public Dictionary<string, long> PlaybackPositions { get; set; } = new Dictionary<string, long>(); //추가
    }
}
