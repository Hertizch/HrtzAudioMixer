using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HrtzAudioMixer.Annotations;

namespace HrtzAudioMixer.Models
{
    public class ActiveApp : INotifyPropertyChanged
    {
        private string _mainWindowTitle;
        private int _processId;
        private string _processName;
        private string _iconPath;
        private float _volumeLevel;
        private bool _isMuted;

        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set
            {
                if (value == _mainWindowTitle) return;
                _mainWindowTitle = value;
                OnPropertyChanged();
            }
        }

        public int ProcessId
        {
            get { return _processId; }
            set
            {
                if (value == _processId) return;
                _processId = value;
                OnPropertyChanged();
            }
        }

        public string ProcessName
        {
            get { return _processName; }
            set
            {
                if (value == _processName) return;
                _processName = value;
                OnPropertyChanged();
            }
        }

        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                if (value == _iconPath) return;
                _iconPath = value;
                OnPropertyChanged();
            }
        }

        public float VolumeLevel
        {
            get { return _volumeLevel; }
            set
            {
                if (Math.Abs(value - _volumeLevel) < 0.001) return;
                _volumeLevel = value;
                OnPropertyChanged();
            }
        }

        public bool IsMuted
        {
            get { return _isMuted; }
            set
            {
                if (value == _isMuted) return;
                _isMuted = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
