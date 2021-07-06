using HAL.Control;
using HAL.Documentation.Base.Monitoring;
using HAL.ImageAnalysis.Implementation.Features;
using HAL.Objects.Mechanisms;
using HAL.Spatial;
using HAL.Units.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.KaplaPlusCamera.Calibration
{
    public class CameraCalibrationData : CalibrationData<MatrixFrame>
    {
        public CameraCalibrationData(MatrixFrame value) : base(value) { }

        internal CameraCalibrationData(CameraCalibrationData clonee) : this(clonee.Value.Clone()) { }

        ///<inheritdoc/>
        public override SettingsBase Clone() => new CameraCalibrationData(this);
    }

    /// <summary> Calibrate an Camera world reference so it match the robot one.</summary>
    public class CameraCalibration : ControllerCalibration<CameraCalibrationData>
    {
        public CameraCalibration(string alias, Controller primaryController, List<Controller> secondaryControllers, string folderPath) :
            base(alias, primaryController, secondaryControllers, folderPath)
        {
        }

        public CameraCalibration(string alias, Controller primaryController, Controller secondaryControllers, string folderPath) :
            base(alias, primaryController, secondaryControllers, folderPath)
        {
        }

        public CameraCalibration(string alias, List<Controller> secondaryControllers, Time tick, string folderPath) :
            base(alias, secondaryControllers, tick, folderPath)
        {
        }


        ///<inheritdoc />
        protected override Task<CameraCalibrationData> Solve()
        {
            List<MechanismState> robotStates = new List<MechanismState>();
            List<CameraEventArg> cameraStates = new List<CameraEventArg>();
            foreach (StateRecord stateRecord in Monitor.Recorder.Records.OfType<StateRecord>())
            {
                foreach (var state in stateRecord.States)
                {
                    switch (state)
                    {
                        case MechanismState mechanismState:
                            robotStates.Add(mechanismState);
                            break;
                        case CameraEventArg cameraState:
                            cameraStates.Add(cameraState);
                            break;
                    }
                }
            }
           List<Marker> markers= (List<Marker>) cameraStates.Select(c => c.Features.OfType<Marker>());
            MatrixFrame calibrationMatrix = Vector3D.FindTransformation(
               markers.Select(c=> c.Position).ToList(),
                robotStates.Select(c => c.Tool.EndPointPosition.LocationInWorld(false).Position).ToList());

            return Task.FromResult(new CameraCalibrationData(calibrationMatrix));
        }
    }
}
