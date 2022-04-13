using HAL.ABB.Control;
using HAL.ABB.Control.Subsystems.EGM;
using HAL.ATI.Control;
using HAL.ATI.Control.Subsystems;
using HAL.Control;
using HAL.Control.Subsystems.Communication;
using HAL.Documentation.Base;
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
        public static async Task Main()
        {
            MatrixFrame coordinateSytem = MatrixFrame.Identity.RotateAroundWorldZ(2);

            await Run((kg)10, MatrixFrame.Identity, MatrixFrame.Identity, coordinateSytem);
        }

        public static async Task Run(Mass sensorMass, MatrixFrame frame, MatrixFrame sensorCenterOfMass, MatrixFrame sensorCoordinateSytem, string sensorIPAddress = "192.168.1.205", string listenerIPAddress = "192.168.1.184", string remoteControllerIPAddress = "192.168.1.202")
        {


            var client = new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(NetBoxManager)), Assembly.GetAssembly(typeof(ABBController)));
            await client.StartAsync();

            var virtualCell = new AtiVirtualCell(client, @"C:\Users\ThomasDelaplanche\Documents\HAL\Exports\SerializedSessions\TestSession.hal");
            virtualCell.CommunicationSettings.IpSettings.Add(new IpSettings("SensorAddress", sensorIPAddress, 49152));
            virtualCell.CommunicationSettings.IpSettings.Add(new IpSettings("ListenerAddress", listenerIPAddress, 60041));
            virtualCell.CommunicationSettings.IpSettings.Add(new IpSettings("RemoteControllerAddress", remoteControllerIPAddress));

            virtualCell.ForceSensorSettings = new ForceSensorSettings(frame,sensorCoordinateSytem,sensorMass,sensorCenterOfMass);
            virtualCell.InitializeForce();

            var mechanism = virtualCell.Mechanism;
            var robotController = virtualCell.Controller;

            var monitor = new Monitor("Monitors the force sensor values and the robot's position", new List<IStateReceivingSubsystem> { virtualCell.AtiManager }, virtualCell.EgmManager, "");
            monitor.StateChanged += OnMonitorStateChanged;
            monitor.Start();
            GetState(virtualCell.AtiManager);
            await Task.Delay(10000);
            monitor.Stop();

            //Todo: show how to save data from monitor

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
}

