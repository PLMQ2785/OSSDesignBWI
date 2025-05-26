using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface IMediaFileController
    {
        string? SelectMediaFile();
        IEnumerable<string>? SelectMediaFiles(); // 여러 파일 선택 메서드
    }
}
