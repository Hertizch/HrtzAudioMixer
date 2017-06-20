using System.Windows;
using System.Windows.Controls;

namespace HrtzAudioMixer.Controls
{
    /// <summary>
    /// Interaction logic for AudioDeviceControl.xaml
    /// </summary>
    [TemplatePart(Name = "PART_Slider", Type = typeof(Slider))]
    public partial class AudioDeviceControl : UserControl
    {
        public AudioDeviceControl()
        {
            InitializeComponent();
        }

        public object ApplicationName
        {
            get { return (string)GetValue(ApplicationNameProperty); }
            set { SetValue(ApplicationNameProperty, value); }
        }

        public object VolumeLevel
        {
            get { return (double)GetValue(VolumeLevelProperty); }
            set { SetValue(VolumeLevelProperty, value); }
        }

        public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(string),
            typeof(AudioDeviceControl), new PropertyMetadata(null));

        public static readonly DependencyProperty VolumeLevelProperty = DependencyProperty.Register("VolumeLevel", typeof(double),
            typeof(AudioDeviceControl), new PropertyMetadata(null));
    }
}
