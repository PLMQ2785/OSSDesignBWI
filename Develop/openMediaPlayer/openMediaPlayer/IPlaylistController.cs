using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer
{
    public interface IPlaylistController
    {
        //재생 목록 변경?
        event EventHandler? PlaylistUpdated;

        //재생 트랙 변경?
        event EventHandler<MediaItem?>? CurrentTrackChanged;

        //재생 목록 가져오기
        IEnumerable<MediaItem> CurrentPlaylist { get; }

        //재생중 트랙 가져오기
        MediaItem? CurrentTrack { get; }

        /// 재생 목록 반복 여부를 설정하거나 가져옵니다.
        bool IsRepeatEnabled { get; set; }

        /// 단일 미디어를 재생 목록에 추가합니다.
        void AddMedia(string filePath);

        /// 여러 미디어를 재생 목록에 추가합니다.
        void AddMultipleMedia(IEnumerable<string> filePaths);

        /// 재생 목록에서 특정 미디어를 제거합니다.
        void RemoveMedia(MediaItem item);

        /// 재생 목록을 모두 지웁니다.
        void ClearPlaylist();

        /// 지정된 트랙을 재생합니다.
        void PlayTrack(MediaItem item);
        //Task PlayTrack(MediaItem item);

        /// 다음 트랙을 재생합니다.
        void NextTrack();

        /// 이전 트랙을 재생합니다.
        void PreviousTrack();

        // (선택 사항) 재생 목록 저장 및 로드
        // Task<bool> LoadPlaylistAsync(string filePath);
        // Task<bool> SavePlaylistAsync(string filePath);
    }
}
