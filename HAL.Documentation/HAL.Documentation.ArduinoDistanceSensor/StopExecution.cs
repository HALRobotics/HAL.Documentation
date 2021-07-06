using System;
using System.Threading.Tasks;
using HAL.Alerts;

namespace HAL.Documentation.KaplaPlusDistanceSensor
{
    public static class StopExecution
    {
        /// <summary>Simple test sensor function. </summary>
        /// <returns>Completed task.</returns>
        public static async Task Main(int sensorCount, string distanceSensorCOM)
        {
            // test sensor
            var sensor = new DistanceSensorManager(sensorCount, distanceSensorCOM);
            sensor.StateUpdated -= OnStateUpdated;
            sensor.StateUpdated += OnStateUpdated;
            sensor.StopReceived -= OnStopReceived;
            sensor.StopReceived += OnStopReceived;
            ConsoleClient.Logger.AddLine = false;

            // add robot web services subsystem
            var rws = Helpers.AddRwsManager(controller, "127.0.0.1");
            var egm = Helpers.AddEgmSubsystem(controller, "127.0.0.1", 6510);

            sensor.Start();
            Console.ReadLine();
            ConsoleClient.Logger.AddLine = true;

        }

        public static int Threshold { get; set; } = 10;
        private static void OnStopReceived(object sender, EventArgs e)
        {
            if (sender is DistanceSensorManager sensor) sensor.Stop();
            ConsoleClient.Logger.AddLine = true;
            ConsoleClient.Logger.Log(new Alert("Stop", AlertLevel.Warning, "Stop", "Sensor will now stop."));
        }

        private static void OnStateUpdated(DistanceSensorManager sender, DistanceSensorEventArg e)
        {
            ConsoleClient.Logger.Log(new[] { $"{e.Data}" });
            if (sender.DistanceSensors[0].Value <= 10)
            {
                //Todo add rwsSignal for Trap
            }
        }
    }
}