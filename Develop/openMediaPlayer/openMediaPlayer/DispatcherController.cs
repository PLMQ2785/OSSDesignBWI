using openMediaPlayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace openMediaPlayer.Services
{
    public class DispatcherController : IDispatcherController
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherController()
        {
            //UI Thread에서 생성 or Main App의 Dispatcher를 사용
            _dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        public void Invoke(Action action)
        {
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _dispatcher.Invoke(action);
            }
        }

        public Task InvokeAsync(Action action)
        {
            //InvokeAsync는 DispatcherOperation을 반환하므로 Task로 변환
            return _dispatcher.InvokeAsync(action).Task;
        }
    }
}
