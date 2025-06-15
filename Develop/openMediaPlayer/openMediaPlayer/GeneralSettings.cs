using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Models
{
    public class GeneralSettings
    {
        public string Language { get; set; } = "en-US"; // 기본 언어
        public bool RememberLastPosition { get; set; } = true; // 종료 시 마지막 위치 기억
    }
}
