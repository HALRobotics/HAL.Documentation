using HAL.ABB.Control;
using HAL.ABB.Control.Subsystems.RobotWebServices;
using HAL.ABB.Helpers;
using HAL.Communications;
using HAL.Control;
using HAL.Control.Subsystems.Procedures;
using HAL.Objects.Mechanisms;
using HAL.Runtime;
using HAL.Units.Electrical;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HAL.Documentation.ArduinoDistanceSensor
{
    class ReadDistanceAndStop
    {
        #region Properties
        public static ElectricSignal StopSignal { get; set; }
        public static RobotWebServicesManager RwsManager { get; set; }
        #endregion

        /// <summary>Simple test sensor function.</summary>
        /// <returns>Completed task.</returns>
        public static async Task Run(string RwsIPAddress, string distanceSensorCOM, string stopSignalId, double threshold )
        {
            var client = new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(ABBController)));
            await client.StartAsync();

            // distanceSensor
            var sensor = new DistanceSensorManager(distanceSensorCOM);
            sensor.StateUpdated -= OnStateUpdated;
            sensor.StateUpdated += OnStateUpdated;


            // add robot web services subsystem
            RwsManager = new RobotWebServicesManager(IPAddress.Parse("192.168.1.202")); //todo input

            /// Create an helper for session management. A path to a serialized session can be input. By default a session saved in the project will be used.
            var sessionHelper = new SessionHelper();
            sessionHelper.TryDeserializeSession(out var robotController, out var mechanism);

            robotController.SubsystemManager.Add(RwsManager);
            StopSignal = new ElectricSignal(new Identifier(stopSignalId), SignalQuantization.Digital, Direction.Out, 0, 24);
            await RwsManager.TryMapSignalAsync(StopSignal);

            await RwsManager.StartMonitoringSignals();
            sensor.Start();
            await RwsManager.TrySetSignalValueAsync(StopSignal, true);
            await RwsManager.TrySetSignalValueAsync(StopSignal, false);
            await RwsManager.TrySetSignalValueAsync(StopSignal, true);
            Console.ReadLine();

        }

        public static int Threshold { get; set; } = 10;


        private static void OnStateUpdated(DistanceSensorManager sender, DistanceSensorEventArg e)
        {
            if (sender.Value > Threshold) Console.WriteLine($"Distance =  {sender.Value}");

            else
            {
                Console.WriteLine($"Too close");
                RwsManager.TrySetSignalValueAsync(StopSignal, (V)24);
            }
        }

    }
}


class SessionHelper

{
    #region Constructors

    /// <summary>
    /// Deserialize a cell from a path and select the first controller and mechanism.
    /// </summary>
    /// <param name="serializedSessionPath">Path from the serialized cell.</param>
    public SessionHelper(string serializedSessionPath = null)
    {
        SerializedSessionPath = serializedSessionPath;
    }

    #endregion

    #region Properties
    public Session Session { get; set; }
    public string SerializedSessionPath { get; set; }

    #endregion

    #region Methods

    /// <summary> Deserialize a session and extract the controller and mechanism.</summary>
    public bool TryDeserializeSession(out RobotController controller, out Mechanism mechanism)
    {
        controller = null;
        mechanism = null;

        //Retrieve session information
        if (string.IsNullOrEmpty(SerializedSessionPath) || !File.Exists(SerializedSessionPath))
        {
            SerializedSessionPath = string.Empty;
            Console.WriteLine($"Non-existent or no session file specified. Searching for first local session.");
            var foundSession = Directory.EnumerateFiles("./", "*.hal").FirstOrDefault();
            if (!string.IsNullOrEmpty(foundSession)) SerializedSessionPath = foundSession;
            if (string.IsNullOrEmpty(SerializedSessionPath))
            {
                Console.WriteLine("No session found. Cannot publish data.");

                return false;
            }
        }
        Console.WriteLine($"Loading session from {SerializedSessionPath}");
        Session = HAL.Serialization.Helpers.DeserializeSession(SerializedSessionPath, true);
        if (Session != null)
        {
            controller = Session.ControlGroup.Controllers.OfType<RobotController>().First();
            mechanism = controller.Controlled.OfType<Mechanism>().First();
            controller.AddControlledObject(mechanism);
            return true;
        }
        return false;
    }

    #endregion

}
