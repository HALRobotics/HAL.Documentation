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
    /// <summary>
    /// This is an example on how to use a 6DOF force sensor such as the ATI sensor with a correction on the gravity forces applied to the sensor.
    /// Prior to that the sensor must be calibrated. To apply and maintain the calibration, the orientation of the sensor in the world must be known.
    /// For this example, using an ATI sensor and an ABB robot, the position of the robot and therefore of the sensor is communicated by EGM. 
    /// The values of the sensor come directly form the sensor' controller. The values of the ATI sensor can also be communicated by <see cref= HAL.Documentation.ATI"/>.
    class DataAcquisitionViaSensor
    {
        public static async Task Main() { await Run((kg)0, MatrixFrame.Identity, MatrixFrame.Identity); }

        public static async Task Run(Mass sensorMass, MatrixFrame sensorCenterOfMass, MatrixFrame sensorCoordinateSytem, string sensorIpAdress = "192.168.1.205", string listenerIPAdress = "192.168.1.184", string remoteControllerIpAdress = "192.168.1.202")
        {
            //var acquisition = new DataAcquisitionViaSensor(); //todo is this ok?

            var client = new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(NetBoxManager)), Assembly.GetAssembly(typeof(ABBController)));
            await client.StartAsync();

            ///converts the strings in IpAdresses.
            var iPRemoteControllerIpAdress = IPAddress.Parse(remoteControllerIpAdress);
            var iPListenerIPAdress = IPAddress.Parse(listenerIPAdress);
            var iPSensorIpAdress = IPAddress.Parse(sensorIpAdress);

            DeserializeSession(out var robotController, out var mechanism);

            SetSensor(sensorCoordinateSytem, sensorMass, sensorCenterOfMass, iPSensorIpAdress, iPListenerIPAdress, out var aTIManager, out var sensor);
            var atiController = new ATIController();
            atiController.AddControlledObject(sensor);
            atiController.SubsystemManager.Add(aTIManager);


            ///Attach the sensor to the robot.
            mechanism.AddSubMechanism(sensor, mechanism.ActiveEndPoint, out _, Persistence.Permanent); //todo doc it in simpleApp
            mechanism.SetEndEffector(sensor); //todo: should the sensor be the end effector? 

            ///creates a new EGMManager and add it to the robot.
            var egmManager = new EGMManager(iPListenerIPAdress, 6510, mechanism);
            robotController.SubsystemManager.Add(egmManager);

            var monitor = new Monitor("Monitors the force sensor values and the robot's position", new List<IStateReceivingSubsystem> { aTIManager }, egmManager, "");
            monitor.StateChanged += OnMonitorStateChanged;
            monitor.Start();
            await Task.Delay(10000);
            monitor.Stop();

        }



        private static void EgmManager_StateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is EGMState state) Console.WriteLine(state.Tool.EndPointPosition.LocationInWorld(true));
        }

        private static void ATIManager_StateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is ForceSensor6DofState state) Console.WriteLine(state.Force);

        }



        /// <summary> Creates the force sensor and it's manager.</summary>
        public static void SetSensor(MatrixFrame sensorCoordinateSytem, Mass sensorMass, MatrixFrame sensorCenterOfMass, IPAddress sensorIp, IPAddress listenerIp, out NetBoxManager manager, out ForceSensor6Dof sensor, int senderPort = 49152, int receiverPort = 60041) //todo revieuw in/output order
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
        public static void DeserializeSession(out RobotController controller, out Mechanism mechanism)
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
                            if (subsystem is NetBoxManager netBoxManager) // Todo : make every Force Sensor Manager an IForceSensorManager
                            {
                                if (netBoxManager.State is ForceSensor6DofState forceSensor6DofState) Console.WriteLine(forceSensor6DofState.Force);
                            }
                        }
                    }

                }

                //  var forceState = forceStates.FirstOrDefault();//.CorrectedForce;

            }
        }
    }
}
