using HAL.ABB.Control.Subsystems.EGM;
using HAL.ATI.Control;
using HAL.ATI.Control.Subsystems;
using HAL.Documentation.Base;
using HAL.Graphs;
using HAL.Objects.Sensors.Force;
using HAL.Runtime;
using HAL.Spatial;
using HAL.Units.Mass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HAL.Documentation.ATI
{
    public class AtiVirtualCell : VirtualCell
    {
        #region Construtors

        public AtiVirtualCell(Client client, string serializedSessionPath) : base(client, serializedSessionPath)
        {
            ForceSensorSettings = new ForceSensorSettings();
        }

        public AtiVirtualCell(Client client, string serializedSessionPath, MatrixFrame frame, MatrixFrame sensorCoordinateSytem, Mass sensorMass, MatrixFrame sensorCenterOfMass) : base(client, serializedSessionPath)
        {

            ForceSensorSettings = new ForceSensorSettings(frame, sensorCoordinateSytem, sensorMass, sensorCenterOfMass);

        }

        #endregion

        ForceSensor6Dof _forceSensor = null;
      
        #region Properties

        public NetBoxManager AtiManager { get; set; }
        public ATIController AtiController { get; private set; }
        public ForceSensor6Dof ForceSensor { get => _forceSensor; private set { _forceSensor = value; } }
        public ForceSensorSettings ForceSensorSettings { get; set; }

        public EGMManager EgmManager { get; set; }

        #endregion

        #region Methods

        public void InitializeForce()
        {
            InitializeForceSensor();
            InitializeNetBoxManager();
            InitializeAtiController();
        }

       public void InitalizeEGM()
        {
            EgmManager = new EGMManager(CommunicationSettings.IpSettings.First(x => x.Alias == "ListenerAddress").IpAddress, CommunicationSettings.IpSettings.First(x => x.Alias == "ListenerAddress").Port.GetValueOrDefault(6510), Mechanism); //Todo: ok to do this null chck whith port?
            Controller.SubsystemManager.Add(EgmManager);
        }

        private void InitializeForceSensor()
        {
            ForceSensor6Dof.Create(ref _forceSensor, "AtiSensor", ForceSensorSettings.Frame, ForceSensorSettings.SensorCoordinateSytem, null, null, ForceSensorSettings.SensorMass, ForceSensorSettings.SensorCenterOfMass, MatrixFrame.Identity, out _forceSensor);
            AddSensorToMechanism();
        }

        private void AddSensorToMechanism()
        {
            Mechanism.AddSubMechanism(ForceSensor, Mechanism.ActiveEndPoint, out _, Persistence.Permanent); //todo doc it in simpleApp
            Mechanism.SetEndEffector(ForceSensor);
        }

        private void InitializeNetBoxManager()
        {
            AtiManager = AtiManager ?? new NetBoxManager();
            AtiManager.TrySetNetworkIdentity(this.CommunicationSettings.IpSettings.First(x => x.Alias == "ListenerAddress").CompleteIpAddress);
            AtiManager.TrySetSensorNetworkIdentity(this.CommunicationSettings.IpSettings.First(x => x.Alias == "SensorAddress").CompleteIpAddress);
        }
                
        private void InitializeAtiController()
        {
            AtiController = AtiController ?? new ATIController();
            AtiController.AddControlledObject(ForceSensor); //TODO: is it necessary? Mechanism already a controlled object and sensor attached to Mechanism.
            AtiController.SubsystemManager.Add(AtiManager);
        }

        #endregion

    }

}

