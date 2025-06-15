using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services
{
    public class PlayerActionRegistry : IPlayerActionRegistry
    {
        //컨트롤러 추가
        private readonly IMediaPlayerController _mediaPlayerController;
        private readonly IPlaylistController _playlistController;
        private readonly ISubtitleController _subtitleController;

        // 액션 이름과 실제 실행될 메서드를 매핑하는 딕셔너리를 만들어두자
        private readonly Dictionary<string, Func<Dictionary<string, object>, Task<string>>> _actions = new();

        public PlayerActionRegistry(
            IMediaPlayerController mediaPlayerController,
            IPlaylistController playlistController,
            ISubtitleController subtitleController)
        {
            _mediaPlayerController = mediaPlayerController;
            _playlistController = playlistController;
            _subtitleController = subtitleController;

            RegisterDefaultActions();
        }

        // 액션을 등록
        private void RegisterDefaultActions()
        {
            RegisterAction("play", PlayAction);
            RegisterAction("pause", PauseAction);
            RegisterAction("stop", StopAction);
            RegisterAction("next_track", NextTrackAction);
            RegisterAction("previous_track", PreviousTrackAction);
            RegisterAction("generate_subtitles", GenerateSubtitlesAction);
            //나중에 액션 추가하려면 파라미터 있는 액션을 넣으면 됨, 생각나는건 seek이랑 set_volume 정도..
        }

        public void RegisterAction(string actionName, Func<Dictionary<string, object>, Task<string>> actionDelegate)
        {
            _actions[actionName.ToLower()] = actionDelegate;
        }

        public async Task<string> ExecuteActionAsync(string actionName, Dictionary<string, object> parameters)
        {
            if (_actions.TryGetValue(actionName.ToLower(), out var action))
            {
                try
                {
                    return await action(parameters);
                }
                catch (Exception ex)
                {
                    return $"Error executing action '{actionName}': {ex.Message}";
                }
            }
            return $"Unknown action: '{actionName}'";
        }

        // 개별 액션 구현부

        private Task<string> PlayAction(Dictionary<string, object> parameters)
        {
            _mediaPlayerController.Play();
            return Task.FromResult("재생 합니다.");
        }

        private Task<string> PauseAction(Dictionary<string, object> parameters)
        {
            _mediaPlayerController.Pause();
            return Task.FromResult("일시정지 합니다.");
        }

        private Task<string> StopAction(Dictionary<string, object> parameters)
        {
            _mediaPlayerController.Stop();
            return Task.FromResult("정지 합니다.");
        }

        private Task<string> NextTrackAction(Dictionary<string, object> parameters)
        {
            _playlistController.NextTrack();
            return Task.FromResult("다음 항목을 재생합니다.");
        }

        private Task<string> PreviousTrackAction(Dictionary<string, object> parameters)
        {
            _playlistController.PreviousTrack();
            return Task.FromResult("이전 항목을 재생합니다.");
        }

        private async Task<string> GenerateSubtitlesAction(Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(_mediaPlayerController.CurrentMediaPath))
            {
                return "불러온 미디어가 없습니다.";
            }
            // 이 액션은 비동기이므로 await를 사용합니다.
            await _subtitleController.GenerateAndLoadSubtitlesAsync(_mediaPlayerController.CurrentMediaPath, null);
            return "자막을 생성합니다.";
        }
    }
}
