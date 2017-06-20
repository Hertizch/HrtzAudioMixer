using System.Timers;
using System.Windows.Input;

namespace HrtzAudioMixer.Helpers
{
    public class TimerHelper
    {
        public static void BeginIntervalTimer(int interval, ICommand command, object commandParameter = null, bool executeOnStart = true)
        {
            // Start command on start
            if (executeOnStart)
                if (command.CanExecute(commandParameter))
                    command.Execute(commandParameter);

            var timer = new Timer(interval);

            timer.Elapsed += (sender, args) =>
            {
                if (command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
            };

            timer.Start();
        }
    }
}
