using HAL.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.Base
{
   public class Logger : ILogger
    {
        public bool IsActive { get; set; } = true;

        public bool Log(Alert alert)
        {
            if (alert is null ) return false;
            return Log(alert as Exception);
        }

        private static bool Log(Exception exception)
        {
            Console.WriteLine(exception.ToString());
            return true;
        }

        public bool Log(string message )
        {
            Console.WriteLine(message);
            return true;
        }

        public static Alert SolvingStarted() => new Alert("Solving", AlertLevel.Info, "Solving info", "Solving Started");
        public static Alert SolvingCompleted() => new Alert("Solving", AlertLevel.Info, "Solving info", "Solving Completed");
        public static Alert PromptConfirmationAlert(string topic) => new Alert("", AlertLevel.Info, $"Load {topic} ?", $"Press [y/n]");

    }
}
