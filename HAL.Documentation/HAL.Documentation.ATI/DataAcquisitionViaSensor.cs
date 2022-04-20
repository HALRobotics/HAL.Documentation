using HAL.ABB.Control;
using HAL.ABB.Control.Subsystems.EGM;
using HAL.ATI.Control;
using HAL.ATI.Control.Subsystems;
using HAL.Control;
using HAL.Control.Subsystems.Communication;
using HAL.Documentation.Base;
using HAL.Documentation.Base.Monitoring;
using HAL.Graphs;
using HAL.Objects;
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

            string serializedCellPath = @"C:\Users\ThomasDelaplanche\Documents\HAL\Exports\SerializedSessions\TestSession.hal";

            var forceSensorSettings = new ForceSensorSettings(MatrixFrame.Identity, MatrixFrame.Identity.RotateAroundWorldZ(2), (kg)10, MatrixFrame.Identity);

            var sensorIpSettings = new IpSettings("SensorAddress", "192.168.1.205", 49152);
            var listenerIpSettigs = new IpSettings("ListenerAddress", "192.168.1.184", 60041);
            var remoteControllerIpSettings = new IpSettings("RemoteControllerAddress", "192.168.1.202");
            var ipSettings = new List<IpSettings> { sensorIpSettings, listenerIpSettigs, remoteControllerIpSettings };

            await Run(serializedCellPath, forceSensorSettings, ipSettings);
        }

        public static async Task Run(String serializedCellPath, ForceSensorSettings forceSensorSettings, List<IpSettings> ipSettings)
        {

            var client = new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(NetBoxManager)), Assembly.GetAssembly(typeof(ABBController)));
            await client.StartAsync();

            var virtualCell = new AtiVirtualCell(client, serializedCellPath);

            virtualCell.ForceSensorSettings = forceSensorSettings;
            virtualCell.CommunicationSettings.IpSettings.AddRange(ipSettings);
            virtualCell.InitializeForce();

            var monitor = new Monitor("Monitors the force sensor values and the robot's position", new List<IStateReceivingSubsystem> { virtualCell.AtiManager }, virtualCell.EgmManager, "");
            monitor.StateChanged += OnMonitorStateChanged;
            monitor.Start();
            await GetStates(virtualCell.AtiManager);
            await Task.Delay(10000);
            monitor.Stop();
            
            //Todo: show how to save data from monitor

        }
        #region GetState and state event methods
        //todo decide if keeping them or not
        private static void EgmManager_StateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is EGMState state) Console.WriteLine(state.Tool.EndPointPosition.LocationInWorld(true));
        }

        private static void ATIManager_StateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is ForceSensor6DofState state) Console.WriteLine(state.Force);

        }


        private static async Task GetStates(NetBoxManager manager)
        {
            while (true)
            {
                if (manager.State is ForceSensor6DofState state) Console.WriteLine($"{state.Force}{state.EndPointPosition.LocationInWorld(false)}");
                else Console.WriteLine("Not state to display");
                await Task.Delay(500);
            }
        } 
        #endregion


        private static void OnMonitorStateChanged(object sender, EventArgs e)
        {
            if (sender is Monitor monitor)
            {
               ForceSensor6DofState forceSensor6DofState = (ForceSensor6DofState) monitor.GetMonitorState(new List<Type>{ typeof(ForceSensor6DofState)}).FirstOrDefault();
               Console.WriteLine($"Force: {forceSensor6DofState.Force}");
               Console.WriteLine($"Corrected force: {forceSensor6DofState.CorrectedForce}");
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
                            if (subsystem is NetBoxManager netBoxManager)
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

