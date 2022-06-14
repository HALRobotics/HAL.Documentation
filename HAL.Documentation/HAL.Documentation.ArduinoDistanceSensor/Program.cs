using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using HAL.ABB.Control;
using HAL.ABB.Control.Subsystems.RobotWebServices;
using HAL.Communications;
using HAL.Control;
using HAL.Objects.Mechanisms;
using HAL.Runtime;
using HAL.Units.Electrical;

namespace HAL.Documentation.ArduinoDistanceSensor
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            //Modify your inputs here if needed.
            var iPAddress = "127.0.0.1";
            var iPPort = "COM3";
            var StopsSignalsAlias = "DO_Inter";
            var threshold = 10;
            
            //Uncomment an example to test it.
            //await ReadDistance.Run(iPAddress, iPPort);
            await ReadDistanceAndStop.Run(iPAddress, iPPort, StopsSignalsAlias,threshold);
        }


    }
}
