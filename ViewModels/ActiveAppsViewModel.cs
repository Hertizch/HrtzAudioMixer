using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CSCore.CoreAudioAPI;
using HrtzAudioMixer.Annotations;
using HrtzAudioMixer.Extensions;
using HrtzAudioMixer.Models;
using Timer = System.Timers.Timer;

namespace HrtzAudioMixer.ViewModels
{
    public class ActiveAppsViewModel : INotifyPropertyChanged
    {
        public ActiveAppsViewModel()
        {
            // Get active apps
            if (CommandResolveActiveAudioSessions.CanExecute(null))
                CommandResolveActiveAudioSessions.Execute(null);

            // If design mode, do not run timers.
            if (ShellViewModel.DesignMode) return;

            // Get active apps timer
            var getActiveAppsTimer = new Timer(1000);
            getActiveAppsTimer.Elapsed += (sender, args) =>
            {
                // Remove inactive processes
                foreach (
                    var activeApp in
                        ActiveAppsCollection.ToList().Where(x => !ProcessExtensions.ProcessExists(x.ProcessId)))
                {
                    Debug.WriteLine(
                        $"GetActiveApplications - Removing application: '{activeApp.MainWindowTitle}' Id: '{activeApp.ProcessId}'");
                    ActiveAppsCollection.Remove(activeApp);
                }

                if (CommandResolveActiveAudioSessions.CanExecute(null))
                    CommandResolveActiveAudioSessions.Execute(null);
            };
            getActiveAppsTimer.Start();
        }

        // Private Fields
        private ICommand _commandResolveActiveAudioSessions;
        private ICommand _commandSetAudioSessionVolume;

        // Public Fields
        public ObservableList<ActiveApp> ActiveAppsCollection { get; set; } = new ObservableList<ActiveApp>();

        // Commands

        /// <summary>
        /// Command - Resolve all active audio sessions
        /// </summary>
        public ICommand CommandResolveActiveAudioSessions
        {
            get
            {
                return _commandResolveActiveAudioSessions ??
                       (_commandResolveActiveAudioSessions = new RelayCommand(Execute_ResolveActiveAudioSessions, p => true));
            }
        }

        /// <summary>
        /// Command - Set volume level for specified audio session
        /// </summary>
        public ICommand CommandSetAudioSessionVolume
        {
            get
            {
                return _commandSetAudioSessionVolume ??
                       (_commandSetAudioSessionVolume = new RelayCommand(p => Execute_SetAudioSessionVolume(p as ActiveApp), p => p is ActiveApp));
            }
        }

        // Methods

        /// <summary>
        /// Resolve all active audio sessions
        /// </summary>
        /// <param name="obj">Null</param>
        private void Execute_ResolveActiveAudioSessions(object obj = null)
        {
            try
            {
                using (var sessionManager = AudioManager.GetDefaultAudioSessionManager2(DataFlow.Render))
                {
                    using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                    {
                        foreach (var session in sessionEnumerator)
                        {
                            using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                            {
                                // Get process main window title
                                var mainWindowTitle = sessionControl.Process?.MainWindowTitle;
                                if (string.IsNullOrEmpty(mainWindowTitle)) continue;

                                // Get process name
                                var processName = sessionControl.Process?.ProcessName;
                                if (string.IsNullOrEmpty(processName)) continue;

                                // Get process id
                                var processId = sessionControl.ProcessID;
                                if (processId.Equals(0)) continue;

                                // Get process icon
                                var processIcon = sessionControl.IconPath;

                                // Get volume level and is muted
                                float volume;
                                bool isMuted;
                                using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                                {
                                    volume = simpleVolume.MasterVolume;
                                    isMuted = simpleVolume.IsMuted;
                                }

                                // Set new main window title if process id exists and main window title does not match
                                if (ActiveAppsCollection.Any(x => x.ProcessId.Equals(processId) && !x.MainWindowTitle.Equals(mainWindowTitle)))
                                {
                                    Debug.WriteLine($"GetActiveApplications - Renaming application: '{mainWindowTitle}' Id: '{processId}'");

                                    var activeApps = ActiveAppsCollection.Where(x => x.ProcessId.Equals(processId) && !x.MainWindowTitle.Equals(mainWindowTitle));
                                    foreach (var activeApp in activeApps)
                                        activeApp.MainWindowTitle = mainWindowTitle;
                                }

                                // If the application does not exist in the collection, add it
                                if (ActiveAppsCollection.Any(x => x.MainWindowTitle.Equals(mainWindowTitle))) continue;

                                Debug.WriteLine($"GetActiveApplications - Adding application: '{mainWindowTitle}' Id: '{processId}' Volume: '{volume}'");

                                ActiveAppsCollection.Add(new ActiveApp
                                {
                                    MainWindowTitle = mainWindowTitle,
                                    ProcessName = processName,
                                    ProcessId = processId,
                                    IconPath = processIcon,
                                    VolumeLevel = volume,
                                    IsMuted = isMuted
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Set volume level for specified audio session
        /// </summary>
        /// <param name="activeApp">Object</param>
        private static void Execute_SetAudioSessionVolume(ActiveApp activeApp)
        {
            using (var sessionManager = AudioManager.GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    foreach (var session in sessionEnumerator)
                    {
                        using (var session2 = session.QueryInterface<AudioSessionControl2>())
                        {
                            if (!session2.ProcessID.Equals(activeApp.ProcessId)) continue;

                            using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                            {
                                simpleVolume.MasterVolume = activeApp.VolumeLevel;
                                simpleVolume.IsMuted = activeApp.IsMuted;
                            }
                        }
                    }
                }
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
