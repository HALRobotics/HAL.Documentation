using HAL.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.ArduinoDistanceSensor
{
    class ReadDistance
    {
        public static async Task Run(string ipAdress, string distanceSensorCOM)
        {
            var client = new Client(ClientBootSettings.Minimal);
            await client.StartAsync();

            // distanceSensor
            var sensor = new DistanceSensorManager(distanceSensorCOM);
            sensor.StateUpdated -= OnStateUpdated;
            sensor.StateUpdated += OnStateUpdated;
        }

        /// <summary> Action to perform once a new distance is read.</summary>
        /// <param name="sender"> distance sensor containing the value of the distance detected.</param>
        /// <param name="e"></param>
        private static void OnStateUpdated(DistanceSensorManager sender, DistanceSensorEventArg e)
        {
            Console.WriteLine($"Distance =  {sender.Value}mm.");
        }
    }
}
