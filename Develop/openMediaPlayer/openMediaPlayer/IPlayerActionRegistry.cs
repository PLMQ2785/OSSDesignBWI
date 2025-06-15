using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openMediaPlayer.Services.Interfaces
{
    public interface IPlayerActionRegistry
    {
        // 실행 가능한 액션을 등록
        // <param name="actionName"> -> 이렇게 액션 이름
        // <param name="actionDelegate"> -> 파라미터를 받아 결과를 문자열로 반환하는 비동기 델리게이트는 이렇게 해보자
        void RegisterAction(string actionName, Func<Dictionary<string, object>, Task<string>> actionDelegate);

        // 등록된 액션을 이름과 파라미터로 실행
        // <param name="actionName">실행할 액션의 이름
        // <param name="parameters">액션에 전달할 파라미터
        // <returns>액션 실행 결과 메시지 <- 제대로 될까?..
        Task<string> ExecuteActionAsync(string actionName, Dictionary<string, object> parameters);
    }
}
