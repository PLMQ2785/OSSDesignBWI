using Microsoft.Win32;
using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class MediaFileController : IMediaFileController
    {
        public string? SelectMediaFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                //필요한 미디어 파일 포맷 지정
                Filter = "Media Files|*.mp4;*.mkv;*.avi;*.mov;*.wmv;*.flv;*.mp3;*.wav;*.aac;*.ogg",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }

}
