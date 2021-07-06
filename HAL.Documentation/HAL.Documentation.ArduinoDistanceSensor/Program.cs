using System;
using System.Threading.Tasks;
using HAL;

namespace HAL.Documentation.KaplaPlusDistanceSensor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Run("127.0.0.1", 1, "COM3");
        }

        /// <summary>Simple test sensor function. </summary>
        /// <returns>Completed task.</returns>
        public static async Task Run(string ipAdress, int sensorCount, string distanceSensorCOM)
        {
            // test sensor
            var sensor = new DistanceSensorManager(sensorCount, distanceSensorCOM);
            sensor.StateUpdated -= OnStateUpdated;
            sensor.StateUpdated += OnStateUpdated;
            sensor.StopReceived -= OnStopReceived;
            sensor.StopReceived += OnStopReceived;

            // add robot web services subsystem

            sensor.Start();
            Console.ReadLine();

        }

        public static int Threshold { get; set; } = 10;
        private static void OnStopReceived(object sender, EventArgs e)
        {
            if (sender is DistanceSensorManager sensor) sensor.Stop();
        }

        private static void OnStateUpdated(DistanceSensorManager sender, DistanceSensorEventArg e)
        {
            if (sender.DistanceSensors[0].Value <= 10)
            {
                Console.WriteLine("New State");
                Console.WriteLine(sender.DistanceSensors[0].Value);
                //Todo add rwsSignal for Trap
            }
        }
    }
}
  