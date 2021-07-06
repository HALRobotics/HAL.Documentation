using HAL.Alerts;
using HAL.Units.Time;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HAL.Documentation.KaplaPlusCamera.Helpers
{
    public static class Prompt
    {
        //todo finish it and implement it in Core
        public static bool PromptConfirmation(string topic, ConsoleLogger logger = null)
        {
            var Logger = logger ?? new ConsoleLogger();
            Logger.Log(PromptConfirmationAlert(topic));
            ConsoleKey response = Console.ReadKey(false).Key;
            Console.WriteLine();
            return (response == ConsoleKey.Y);
        }

        internal static Alert PromptConfirmationAlert(string topic) => new Alert("", AlertLevel.Info, $"Load {topic} ?", $"Press [y/n]");
    }

    /// <summary> Based implementation for a logger. </summary>
    public abstract class SeverityLogger : ILogger
    {
        /// <summary>Minimum severity level to log.</summary>
        public int Severity { get; set; } = -1;

        /// <inheritdoc/>
        public abstract bool Log(Alert alert);

        /// <summary>Checks whether the logger will log an alert with this level.</summary>
        /// <param name="level">Level to check.</param>
        /// <returns>Whether an alert with the given level will be logged.</returns>
        public virtual bool WillLog(int level) => Severity <= level;

        /// <summary>Checks whether the logger will log an alert with this level.</summary>
        /// <param name="level">Level to check.</param>
        /// <returns>Whether an alert with the given level will be logged.</returns>
        public bool WillLog(AlertLevel level) => WillLog((int)level);

        /// <inheritdoc/>
        public bool IsActive { get; set; } = true;
    }


    /// <summary> Console logger. </summary>
    public class ConsoleLogger : SeverityLogger
    {
        /// <summary>Whether if log is adding a line.</summary>
        public bool AddLine { get; set; } = true;
        private bool IsRedrawing { get; set; }
        private string[] Message { get; set; }
        private bool LineAdded { get; set; } = true;

        /// <inheritdoc />
        public override bool Log(Alert alert)
        {
            if (alert is null || !WillLog(alert.Severity)) return false;
            if (AddLine && !LineAdded) Console.WriteLine("");
            LineAdded = AddLine;
            return Log(alert as Exception, AddLine);
        }

        private static bool Log(Exception exception, bool addLine)
        {
            if (addLine) Console.WriteLine(exception.ToString());
            else
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine(exception);
            }
            if (!(exception.InnerException is null))
                Log(exception.InnerException, true);
            return true;
        }

        public bool Log(string[] message, bool redraw = true)
        {
            IsRedrawing = Message?.Length == message.Length && redraw;

            for (var index = 0; index < message.Length; index++)
            {
                message[index] = message[index] is not null ? Regex.Replace(message[index], @"\s+", " ") : null;
            }

            Message = message;
            Redraw();
            return true;
        }

        internal void Redraw()
        {
            if (!IsRedrawing) IsRedrawing = true;
            else Console.SetCursorPosition(0, Console.CursorTop - Message.Length);

            foreach (var str in Message)
            {
                Console.WriteLine(new String(' ', Console.BufferWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine(str);
            }
        }

    }

    /// <summary> Console logger. </summary>
    public class ConsoleWatchLogger : ILogger
    {
        /// <inheritdoc />
        public bool Log(Alert alert = null)
        {
            alert ??= TimeElapsed((int)Watch.ElapsedMilliseconds);
            return Log(alert as Exception);
        }

        private static bool Log(Exception exception)
        {
            Console.WriteLine(exception.ToString());
            if (!(exception.InnerException is null))
                Log(exception.InnerException);
            return true;
        }

        /// <inheritdoc />
        public bool IsActive { get; set; }

        private Stopwatch Watch { get; set; } = new Stopwatch();

        private Alert TimeElapsed(int elapsed) => new Alert("TimeElapsed", AlertLevel.Info, "Time elapsed", ((ms)elapsed).ToString());
    }
}

