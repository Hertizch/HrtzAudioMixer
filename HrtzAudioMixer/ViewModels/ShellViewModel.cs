using System.ComponentModel;
using System.Windows;
using HrtzAudioMixer.Extensions;

namespace HrtzAudioMixer.ViewModels
{
    public class ShellViewModel
    {
        private RelayCommand _commandCloseApp;

        public static bool DesignMode { get; set; } = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public RelayCommand CommandCloseApp
        {
            get
            {
                return _commandCloseApp ??
                       (_commandCloseApp = new RelayCommand(Execute_CloseApp, p => true));
            }
        }

        private static void Execute_CloseApp(object obj)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
