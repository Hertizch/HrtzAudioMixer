using System.ComponentModel;
using HrtzAudioMixer.Properties;

namespace HrtzAudioMixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
