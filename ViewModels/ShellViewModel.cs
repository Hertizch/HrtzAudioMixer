using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using HrtzAudioMixer.Extensions;

namespace HrtzAudioMixer.ViewModels
{
    public class ShellViewModel
    {
        private ICommand _commandCloseApp;

        public static bool DesignMode { get; set; } = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public ICommand CommandCloseApp
        {
            get
            {
                return _commandCloseApp ??
                       (_commandCloseApp = new RelayCommand(Execute_CloseApp, p => true));
            }
        }

        private void Execute_CloseApp(object obj)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
