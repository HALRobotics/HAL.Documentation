using HAL.ABB.Control;
using HAL.ABB.Control.Subsystems.EGM;
using HAL.ATI.Control;
using HAL.ATI.Control.Subsystems;
using HAL.Control;
using HAL.Control.Subsystems.Communication;
using HAL.Documentation.Base.Monitoring;
using HAL.Graphs;
using HAL.Objects.Mechanisms;
using HAL.Objects.Sensors.Force;
using HAL.OptiTrack.Control;
using HAL.OptiTrack.Control.Subsystems;
using HAL.Runtime;
using HAL.Spatial;
using HAL.Units.Mass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace HAL.Documentation.ATI
{
    class DataAcquisitionViaSensor
    {
        public static async Task Main()
        {
            MatrixFrame frame = MatrixFrame.Identity.RotateAroundWorldZ(2);

            await Run((kg)10, MatrixFrame.Identity, frame);
        }

        public static async Task Run(Mass sensorMass, MatrixFrame sensorCenterOfMass, MatrixFrame sensorCoordinateSytem, string sensorIPAddress = "192.168.1.205", string listenerIPAddress = "192.168.1.184", string remoteControllerIPAddress = "192.168.1.202", string optiTrackIPAddress = "192.100.100.100")
        {
            var acquisition = new DataAcquisitionViaSensor(); 

            var client = new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(NetBoxManager)), Assembly.GetAssembly(typeof(ABBController)));
            await client.StartAsync();

            ///converts the strings in IpAdresses.
            var iPRemoteControllerIPAddress = IPAddress.Parse(remoteControllerIPAddress);
            var iPListenerIPAddress = IPAddress.Parse(listenerIPAddress);
            var iPSensorIPAddress = IPAddress.Parse(sensorIPAddress);

            acquisition.DeserializeSession(out var robotController, out var mechanism);

            acquisition.SetSensor(sensorCoordinateSytem, sensorMass, sensorCenterOfMass, iPSensorIPAddress, iPListenerIPAddress, out var aTIManager, out var sensor);
            var atiController = new ATIController();
            atiController.AddControlledObject(sensor);
            atiController.SubsystemManager.Add(aTIManager);


            ///Attach the sensor to the robot.
            mechanism.AddSubMechanism(sensor, mechanism.ActiveEndPoint, out _, Persistence.Permanent);
            mechanism.SetEndEffector(sensor);

            ///creates a new EGMManager and add it to the robot.
            var egmManager = new EGMManager(iPListenerIPAddress, 6510, mechanism);
            robotController.SubsystemManager.Add(egmManager);

            /// Creates an OptiTrack controller. 
            var optiTrackController = new OptiTrackController();
            var optiTrackManager = new OptiTrackManager();
            optiTrackController.SubsystemManager.Add(optiTrackManager);
            optiTrackManager.TrySetNetworkIdentity(optiTrackIPAddress);

            var monitor = new Monitor("Monitors the sensors", new List<IStateReceivingSubsystem> {optiTrackManager, aTIManager }, egmManager, "");
            monitor.StateChanged += OnMonitorStateChanged;
            monitor.Start();
            await Task.Delay(10000);
            monitor.Stop();

        }


        /// <summary> Creates the force sensor and it's manager.</summary>
        public void SetSensor(MatrixFrame sensorCoordinateSytem, Mass sensorMass, MatrixFrame sensorCenterOfMass, IPAddress sensorIp, IPAddress listenerIp, out NetBoxManager manager, out ForceSensor6Dof sensor, int senderPort = 49152, int receiverPort = 60041) //todo revieuw in/output order
        {
            //Creates a sensor's manager and set the IP addresses. 
            manager = new NetBoxManager();
            manager.TrySetNetworkIdentity($"{listenerIp}:{receiverPort}");
            manager.TrySetSensorNetworkIdentity($"{sensorIp}:{senderPort}");

            ///Create the sensor and set its geometric parameters.
            ForceSensor6Dof instance = null;
            ForceSensor6Dof.Create(ref instance, "AtiSensor", MatrixFrame.Identity, sensorCoordinateSytem, null, null, sensorMass, sensorCenterOfMass, MatrixFrame.Identity, out sensor); ;
            Console.WriteLine(sensor.Identity);
        }

        /// <summary>Deserialize a session and extract the controller and mechanism.</summary>
        public void DeserializeSession(out RobotController controller, out Mechanism mechanism)
        {
            var session = Serialization.Helpers.DeserializeSession(@"C:\Users\ThomasDelaplanche\SerializedDocuments\SessionTestABB.hal", true);
            controller = session.ControlGroup.Controllers.OfType<RobotController>().First();
            mechanism = controller.Controlled.OfType<Mechanism>().First();
            controller.AddControlledObject(mechanism);

        }

        private static async Task GetState(NetBoxManager manager)
        {
            while (true)
            {
                if (manager.State is ForceSensor6DofState state) Console.WriteLine($"{state.Force}{state.EndPointPosition.LocationInWorld(false)}");
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
                            if (subsystem is NetBoxManager netBoxManager && netBoxManager.State is ForceSensor6DofState forceSensor6DofState) // Todo : make every 6DOFForce Sensor Manager an I6DOForceSensorManager?
                            {
                                    Console.WriteLine($"Corrected force: {forceSensor6DofState.CorrectedForce}");
                            }
                            if (subsystem is OptiTrackManager optiTrackManager && optiTrackManager.Controller is OptiTrackController optiTrackController )
                            {
                                Console.WriteLine($"First Body position: {optiTrackController.Bodies.FirstOrDefault().Location}");
                            }

                        }
                    }
                    
                }
            }
        }
    }
}

