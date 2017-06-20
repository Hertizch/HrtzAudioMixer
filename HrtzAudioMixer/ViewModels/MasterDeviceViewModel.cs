using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CSCore.CoreAudioAPI;
using HrtzAudioMixer.Annotations;
using HrtzAudioMixer.Extensions;
using HrtzAudioMixer.Helpers;

namespace HrtzAudioMixer.ViewModels
{
    public class MasterDeviceViewModel : INotifyPropertyChanged
    {
        public MasterDeviceViewModel()
        {
            // If design mode, do not run timers.
            if (ShellViewModel.DesignMode) return;

            // Get master device name
            TimerHelper.BeginIntervalTimer(1000, CommandGetMasterDeviceName);

            // Get master volume
            TimerHelper.BeginIntervalTimer(1000, CommandGetMasterAudioVolume);

            // Get master peak
            TimerHelper.BeginIntervalTimer(100, CommandGetMasterAudioPeak);
        }

        private RelayCommand _commandGetMasterAudioVolume;
        private RelayCommand _commandSetMasterAudioVolume;
        private RelayCommand _commandGetMasterAudioPeak;
        private RelayCommand _commandGetMasterDeviceName;
        private float _masterAudioLevel;
        private float _masterAudioPeak;
        private bool _masterAudioIsMuted;
        private string _masterDeviceName;

        // Properties
        public float MasterAudioLevel
        {
            get => _masterAudioLevel;
            set
            {
                if (Math.Abs(value - _masterAudioLevel) < 0.001) return;
                _masterAudioLevel = value;

                if (CommandSetMasterAudioVolume.CanExecute(null))
                    CommandSetMasterAudioVolume.Execute(null);

                OnPropertyChanged();
            }
        }

        public float MasterAudioPeak
        {
            get => _masterAudioPeak;
            set
            {
                if (Math.Abs(value - _masterAudioPeak) < 0.001) return;
                _masterAudioPeak = value;
                OnPropertyChanged();
            }
        }

        public bool MasterAudioIsMuted
        {
            get => _masterAudioIsMuted;
            set
            {
                if (value == _masterAudioIsMuted) return;
                _masterAudioIsMuted = value;
                OnPropertyChanged();
            }
        }

        public string MasterDeviceName
        {
            get => _masterDeviceName;
            set
            {
                if (value == _masterDeviceName) return;
                _masterDeviceName = value;
                OnPropertyChanged();
            }
        }

        // Commands

        /// <summary>
        /// Command - Get master audio volume level
        /// </summary>
        public RelayCommand CommandGetMasterAudioVolume
        {
            get
            {
                return _commandGetMasterAudioVolume ??
                       (_commandGetMasterAudioVolume = new RelayCommand(Execute_GetMasterAudioVolume, p => true));
            }
        }

        /// <summary>
        /// Command - Set master audio volume level
        /// </summary>
        public RelayCommand CommandSetMasterAudioVolume
        {
            get
            {
                return _commandSetMasterAudioVolume ??
                       (_commandSetMasterAudioVolume = new RelayCommand(Execute_SetMasterAudioVolume, p => true));
            }
        }

        /// <summary>
        /// Command - Gets master audio peak level
        /// </summary>
        public RelayCommand CommandGetMasterAudioPeak
        {
            get
            {
                return _commandGetMasterAudioPeak ??
                       (_commandGetMasterAudioPeak = new RelayCommand(Execute_GetMasterAudioPeak, p => true));
            }
        }

        /// <summary>
        /// Command - Set master audio volume level
        /// </summary>
        public RelayCommand CommandGetMasterDeviceName
        {
            get
            {
                return _commandGetMasterDeviceName ??
                       (_commandGetMasterDeviceName = new RelayCommand(Execute_GetMasterDeviceName, p => true));
            }
        }

        // Methods

        /// <summary>
        /// Gets the master audio volume level
        /// </summary>
        /// <param name="obj"></param>
        private void Execute_GetMasterAudioVolume(object obj)
        {
            using (var device = AudioManager.GetDefaultRenderDevice())
            {
                using (var endpointVolume = AudioEndpointVolume.FromDevice(device))
                {
                    var volume = endpointVolume.GetMasterVolumeLevelScalar();
                    MasterAudioLevel = volume;

                    var isMuted = endpointVolume.GetMute();
                    MasterAudioIsMuted = isMuted;
                }
            }
        }

        /// <summary>
        /// Sets the master audio volume level
        /// </summary>
        /// <param name="obj"></param>
        private void Execute_SetMasterAudioVolume(object obj)
        {
            using (var device = AudioManager.GetDefaultRenderDevice())
            {
                using (var endpointVolume = AudioEndpointVolume.FromDevice(device))
                {
                    endpointVolume.SetMasterVolumeLevelScalar(MasterAudioLevel, Guid.Empty);

                    if (obj == null) return;

                    var isMuted = Convert.ToBoolean(obj);

                    if (endpointVolume.GetMute() && isMuted) return;
                    if (!endpointVolume.GetMute() && !isMuted) return;

                    endpointVolume.SetMuteNative(isMuted, Guid.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the master audio peak level
        /// </summary>
        /// <param name="obj"></param>
        private void Execute_GetMasterAudioPeak(object obj)
        {
            using (var device = AudioManager.GetDefaultRenderDevice())
            {
                using (var meter = AudioMeterInformation.FromDevice(device))
                {
                    MasterAudioPeak = meter.PeakValue * 100;
                }
            }
        }

        /// <summary>
        /// Gets the master device name
        /// </summary>
        /// <param name="obj"></param>
        private void Execute_GetMasterDeviceName(object obj)
        {
            using (var device = AudioManager.GetDefaultRenderDevice())
            {
                MasterDeviceName = device.FriendlyName;
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
