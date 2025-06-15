using openMediaPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace openMediaPlayer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            // ViewModel에서 창을 닫도록 요청하면, 이 코드가 실행됨
            if (viewModel.CloseAction == null)
            {
                viewModel.CloseAction = new System.Action(this.Close);
            }
        }
    }
}
