using HAL.ABB.Control;
using HAL.ABB.Control.Subsystems.EGM;
using HAL.ATI.Control;
using HAL.ATI.Control.Subsystems;
using HAL.Control;
using HAL.Control.Subsystems.Communication;
using HAL.Control.Subsystems.Procedures;
using HAL.Documentation.Base.Monitoring;
using HAL.Graphs;
using HAL.Objects.Mechanisms;
using HAL.Objects.Sensors.Force;
using HAL.Runtime;
using HAL.Spatial;
using HAL.Units.Mass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HAL.Documentation.ATI
{
    /// <summary>
    /// This is an example on how to use a 6DOF force sensor such as the ATI sensor with a correction on the gravity forces applied to the sensor.
    /// Prior to that the sensor must be calibrated. To apply and maintain the calibration, the orientation of the sensor in the world must be known.
    /// For this example, using an ATI sensor and an ABB robot, the position of the robot and therefore of the sensor is communicated by EGM. 
    /// The values of the sensor come directly form the sensor' controller. The values of the ATI sensor can also be communicated by <see cref= HAL.Documentation.ATI"/>.
    class Program
    {
        public static async Task Main()
        {
            MatrixFrame frame = MatrixFrame.Identity.RotateAroundWorldZ(2);

            await Run((kg)10, MatrixFrame.Identity, frame);
        }

        public static async Task Run(Mass sensorMass, MatrixFrame sensorCenterOfMass, MatrixFrame sensorCoordinateSytem, string sensorIPAddress = "192.168.1.205", string listenerIPAddress = "192.168.1.184", string remoteControllerIPAddress = "192.168.1.202")
        {
            var client = new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(NetBoxManager)), Assembly.GetAssembly(typeof(ABBController)));
            await client.StartAsync();

            /// Create an helper for session management. A path to a serialized session can be input. By default a session saved in the project will be used.
            var sessionHelper = new SessionHelper();

            // Convert the strings into an IpEndPoint (IPAddress + Port)
            var RemoteControllerIPEndPoint = IPEndPoint.Parse(remoteControllerIPAddress);
            var listenerIPEndpoint = IPEndPoint.Parse(listenerIPAddress);
            var sensorIPEndpoint = IPEndPoint.Parse(sensorIPAddress); // todo: rename that

            // Use the sessionHelper to deserialize the session 
            sessionHelper.TryDeserializeSession(out var robotController, out var robot);

            // The ForceSensorHelper instantiate a Force
            var forceSensorHelper = new ForceSensorHelper(MatrixFrame.Identity, sensorCoordinateSytem, sensorMass, sensorCenterOfMass);
            forceSensorHelper.InitializeForce(robot, listenerIPEndpoint, sensorIPEndpoint);
            forceSensorHelper.InitalizeEGM(robotController, robot, listenerIPEndpoint);


            // Create a new monitor.
            var monitor = new Base.Monitoring.Monitor("Monitors the force sensor values and the robot's position", new List<IStateReceivingSubsystem> { forceSensorHelper.AtiManager }, forceSensorHelper.EgmManager, "");
            // Subscribe the StateChanged event. OnMonitorStateChanged method will be performed each time a new state enters the monitor.
            monitor.StateChanged += OnMonitorStateChanged;
            monitor.Start();
            await Task.Delay(10000);
            monitor.Stop();
            monitor.Recorder.Save(true, "");
        }

        private static void EgmManager_StateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is EGMState state) Console.WriteLine(state.Tool.EndPointPosition.LocationInWorld(true));
        }

        private static void ATIManager_StateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is ForceSensor6DofState state) Console.WriteLine(state.Force);
        }
        private static async Task GetState(NetBoxManager manager)
        {
            while (true)
            {
                if (manager.State is ForceSensor6DofState state) Console.WriteLine($"{ state.Force}{state.EndPointPosition.LocationInWorld(false)}");
                else Console.WriteLine("Not state to display");
                await Task.Delay(500);
            }
        }

        private static void OnMonitorStateChanged(object sender, EventArgs e)
        {
            if (sender is Monitor monitor)
            {


                foreach (HAL.Control.ControllerState controllerState in monitor.CurrentRecord.States)
                {
                    if (!(controllerState is null))
                    {
                        foreach (HAL.Control.Subsystems.IControllerSubsystem subsystem in controllerState?.Source.SubsystemManager.Subsystems)
                        {
                            if (subsystem is NetBoxManager netBoxManager) // Todo : make every 6DOFForce Sensor Manager an I6DOForceSensorManager?
                            {
                                if (netBoxManager.State is ForceSensor6DofState forceSensor6DofState)
                                {
                                    Console.WriteLine($"Force: {forceSensor6DofState.Force}");
                                    Console.WriteLine($"Corrected force: {forceSensor6DofState.CorrectedForce}");
                                }
                            }

                        }
                    }
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
        public SessionHelper(string serializedSessionPath = null, string exportPath = null) //Todo add: session serilaizer API for public.
        {
            SerializedSessionPath = serializedSessionPath;
            ExportPath = exportPath;
        }

        #endregion

        #region Properties
        public Session Session { get; set; }
        public string SerializedSessionPath { get; set; }

        public string ExportPath { get; set; }

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
            Session = Serialization.Helpers.DeserializeSession(SerializedSessionPath, true);
            if (Session != null)
            {
                controller = Session.ControlGroup.Controllers.OfType<RobotController>().First();
                mechanism = controller.Controlled.OfType<Mechanism>().First();
                controller.AddControlledObject(mechanism);
                return true;
            }
            return false;
        }

        public void Solve(RobotController robotController)
        {
            ///Subscribe to solving events. The logger will display the appropriate alerts when the events are triggered. 
            Session.ControlGroup.Solver.SolvingCompleted += OnSolvingCompleted;
            Session.ControlGroup.Solver.SolvingStarted += OnSolvingStarted;

            ///Start the solving. An unsolved procedure cannot be exported nor uploaded.
            Session.ControlGroup.Solver.StartSolution();

        }

        public async Task Upload(RobotController robotController, string controllerIp)
        {
            Solve(robotController);
            /// Select a subsystem able to export and upload code.
            var canUpload = robotController.SubsystemManager.TryGetSubsystem<ILoadingCapableSubsystem>(out var uploader);
            if (canUpload)
            {
                uploader.TrySetNetworkIdentity(controllerIp);
                await robotController.TryExportAndUploadAsync(Linguistics.Export.DeclarationMode.Inline, false, cancel: CancellationToken.None);
            }
        }

        public async Task Export(RobotController robotController)
        {
            Solve(robotController);
            await robotController.TryExportAsync(ExportPath, Linguistics.Export.DeclarationMode.Inline, cancel: CancellationToken.None);
        }

        #region Events  

        // Actions to performs when the events are triggered 

        private void OnSolvingStarted(object sender, EventArgs e) //Todo is it ok that this events are not static
        {
            Console.WriteLine("SolvingStarted.");
        }

        private void OnSolvingCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("SolvingCompleted.");
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// Help setting the elements to monitor the force sensor data.
    /// </summary>
    class ForceSensorHelper
    {

        #region Constructors

        /// <summary>
        /// Create a new instance of a force
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="sensorCoordinateSytem"></param>
        /// <param name="sensorMass"></param>
        /// <param name="sensorCenterOfMass"></param>
        public ForceSensorHelper(MatrixFrame frame, MatrixFrame sensorCoordinateSytem, Mass sensorMass, MatrixFrame sensorCenterOfMass)
        {
            ForceSensor6Dof.Create(ref _forceSensor, "AtiSensor", frame, sensorCoordinateSytem, null, null, sensorMass, sensorCenterOfMass, MatrixFrame.Identity, out _forceSensor);
        }

        #endregion

        #region Fields

        ForceSensor6Dof _forceSensor = null;

        #endregion

        #region Properties

        public Mechanism SensorParentMechanism { get; set; }
        public NetBoxManager AtiManager { get; set; }
        public ATIController AtiController { get; private set; }
        public ForceSensor6Dof ForceSensor { get => _forceSensor; private set { _forceSensor = value; } }

        public EGMManager EgmManager { get; set; }

        #endregion

        #region Methods

        /// <summary>Initialize all the elements needed to read </summary>
        /// <param name="sensorParentMechanism"></param>
        /// <param name="listenerIpAddress"></param>
        /// <param name="sensorIPAddress"></param>
        public void InitializeForce(Mechanism sensorParentMechanism, IPEndPoint listenerIpAddress, IPEndPoint sensorIPAddress)
        {
            AddSensorToMechanism(sensorParentMechanism);
            InitializeNetBoxManager(listenerIpAddress, sensorIPAddress);
            InitializeAtiController();
        }

        /// <summary>Create a new EGM manager and add it to the controller's subsystems.</summary>
        /// <param name="robotController"></param>
        /// <param name="mechanism"></param>
        /// <param name="listenerIPAddress"></param>
        public void InitalizeEGM(RobotController robotController, Mechanism mechanism, IPEndPoint listenerIPAddress)
        {
            EgmManager = new EGMManager(listenerIPAddress.Address, listenerIPAddress.Port, mechanism);
            robotController.SubsystemManager.Add(EgmManager);
        }

        /// <summary>Attach the sensor to it's parent mechanism. </summary>
        /// <param name="mechanism">Mechanism holding the sensor.</param>
        private void AddSensorToMechanism(Mechanism mechanism)
        {
            mechanism.AddSubMechanism(ForceSensor, mechanism.ActiveEndPoint, out _, Persistence.Permanent); //todo doc it in simpleApp
            mechanism.SetEndEffector(ForceSensor);
        }

        /// <summary>Ensure the force sensor's manager exists and set its IPaddresses.</summary>
        /// <param name="listenerIPAddress"></param>
        /// <param name="sensorIPAddress"></param>
        private void InitializeNetBoxManager(IPEndPoint listenerIPAddress, IPEndPoint sensorIPAddress)
        {
            AtiManager = AtiManager ?? new NetBoxManager();
            AtiManager.TrySetNetworkIdentity(listenerIPAddress.ToString());
            AtiManager.TrySetSensorNetworkIdentity(sensorIPAddress.ToString());
        }

        /// <summary>Ensure the force sensor's controller exists then assign it the force sensor and its manager./// </summary>
        private void InitializeAtiController()
        {
            AtiController = AtiController ?? new ATIController();
            AtiController.AddControlledObject(ForceSensor); //TODO: is it necessary? Mechanism already a controlled object and sensor attached to Mechanism.
            AtiController.SubsystemManager.Add(AtiManager);
        }

        #endregion

    }
}

