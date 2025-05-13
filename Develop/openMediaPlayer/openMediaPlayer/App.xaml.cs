using LibVLCSharp.Shared;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace openMediaPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //기본 프로토타입용 코드
            ////LibVLC Init
            //Core.Initialize();

            //for debug
            //this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        // private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        // {
        //     // 예외 로깅 또는 사용자 알림
        //     MessageBox.Show("처리되지 않은 오류가 발생했습니다: " + e.Exception.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        //     e.Handled = true; // true로 설정하면 애플리케이션이 종료되지 않음!
        // }
    }

}
