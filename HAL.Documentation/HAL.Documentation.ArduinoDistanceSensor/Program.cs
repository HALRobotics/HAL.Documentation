using System;
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
    class Program
    {
        #region Properties
        public static ElectricSignal StopSignal { get; set; }
        public static RobotWebServicesManager RwsManager { get; set; }
        #endregion
        static async Task Main(string[] args)
        {
            await Run("127.0.0.1", 1, "COM3", "DO_Inter");
        }

        /// <summary>Simple test sensor function.</summary>
        /// <returns>Completed task.</returns>
        public static async Task Run(string ipAdress, int sensorCount, string distanceSensorCOM, string stopSignalId)
        {
            var client = new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(ABBController)));
            await client.StartAsync();

            // distanceSensor
            var sensor = new DistanceSensorManager(sensorCount, distanceSensorCOM);
            sensor.StateUpdated -= OnStateUpdated;
            sensor.StateUpdated += OnStateUpdated;


            // add robot web services subsystem
            RwsManager = new RobotWebServicesManager(IPAddress.Parse("192.168.1.202"));
            DeserializeSession(out var robotController, out var mechanism);
            robotController.SubsystemManager.Add(RwsManager);
            StopSignal = new ElectricSignal(new Identifier(stopSignalId), SignalQuantization.Digital, Direction.Out, 0,24);
            await RwsManager.TryMapSignalAsync(StopSignal);

            await RwsManager.StartMonitoringSignals();
            sensor.Start();
            await RwsManager.TrySetSignalValueAsync(StopSignal,true);
            await RwsManager.TrySetSignalValueAsync(StopSignal, false);
            await RwsManager.TrySetSignalValueAsync(StopSignal, true);
            Console.ReadLine();

        }

        public static int Threshold { get; set; } = 10;


        private static void OnStateUpdated(DistanceSensorManager sender, DistanceSensorEventArg e)
        {
            if (sender.DistanceSensors[0].Value > 5) Console.WriteLine($"Distance =  {sender.DistanceSensors[0].Value}");

            else
            {
                Console.WriteLine($"Too close");
                RwsManager.TrySetSignalValueAsync(StopSignal, (V)24);
            }
        }

        /// <summary>Deserialize a session and extract the controller and mechanism.</summary>
        public static void DeserializeSession(out RobotController controller, out Mechanism mechanism)
        {

            var session = Serialization.Helpers.DeserializeSession(@"C:\Users\ThomasDelaplanche\SerializedDocuments\SessionTestABB.hal", true);
            controller = session.ControlGroup.Controllers.OfType<RobotController>().First();
            mechanism = controller.Controlled.OfType<Mechanism>().First();
            controller.AddControlledObject(mechanism);

        }
    }
}
