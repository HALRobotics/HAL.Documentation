
using HAL.ABB.Control;
using HAL.Communications;
using HAL.Control;
using HAL.Control.Subsystems.Communication;
using HAL.Documentation.Base;
using HAL.ImageAnalysis.Implementation.Features.Sources;
using HAL.Objects.Mechanisms;
using HAL.OptiTrack.Control;
using HAL.OptiTrack.Control.Subsystems;
using HAL.Runtime;
using HAL.Units.Electrical;
using HAL.Units.Length;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.KaplaPlusCamera.Calibration
{
   class RunCalibration
    {
       static Logger InfoLogger = null;
  public  static async Task Run()
        {
            ///Create a new client and set the required assemblies. Mandatory step. 
            var client = new Client(ClientBootSettings.Minimal, 
                Assembly.GetAssembly(typeof(IStateReceivingSubsystem)),
                Assembly.GetAssembly(typeof(ABBController))
                //,Assembly.GetAssembly(typeof(OptiTrackController))
                );
            await client.StartAsync();
            

            InfoLogger = new Logger();

            #region Session and Cell

            ///Deserialize a session and the cell components.
            var session = Serialization.Helpers.DeserializeSession(@"C:\Users\ThomasDelaplanche\SerializedDocuments\SessionTestABB.hal", true);
            var controller = session.ControlGroup.Controllers.OfType<RobotController>().First();
            var mechanism = controller.Controlled.OfType<Mechanism>().First();
            var cameraController = new Controller(new Identifier("cameraController"),new List<Protocol>());
            //var optitrack = new OptiTrackController();
           // var optiManager = new OptiTrackManager();
            var cameraManager = new CameraManager((mm)2,0);
            cameraController.SubsystemManager.Add(cameraManager);
            //optitrack.SubsystemManager.Add(optiManager);

            var canRecieveStates = controller.SubsystemManager.TryGetSubsystem<IStateReceivingSubsystem>(out var stateReciever);
            if (canRecieveStates)
            {
                stateReciever.TrySetNetworkIdentity("192.168.0.103"); /// Set the real controller Ip adress.
                
            }
            #endregion

            Console.WriteLine("test");
            var cameraCalibration = new CameraCalibration("Camera calibration", controller, cameraController, "");
            await cameraCalibration.CalibrateAsync(new CalibrationSettings(), false,new HAL.Tasks.BackgroundProgress(), new System.Threading.CancellationToken());
            
        }
    }

    public class CalibrationSettings : ICalibrationSettings
    {
       public  CalibrationSettings() 
        {
            ConnectTrigger = new ElectricSignal(new Identifier("ConnectTrigger"),SignalQuantization.Digital,Direction.Out,(V)24);
            RecordTrigger = new ElectricSignal(new Identifier("RecordTrigger"),SignalQuantization.Digital,Direction.Out,(V)24);
        }
       CalibrationSettings(string alias,ElectricSignal connectTrigger, ElectricSignal recordTrigger) 
        {
            Identity = new Identifier(alias);
            ConnectTrigger = connectTrigger;
            RecordTrigger = recordTrigger;
        }

        
        public ElectricSignal ConnectTrigger { get; }

        public ElectricSignal RecordTrigger { get; }

        public Identifier Identity { get; }

        public bool Equals(IIdentifiable other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Identifier other)
        {
            throw new NotImplementedException();
        }
    }
}
