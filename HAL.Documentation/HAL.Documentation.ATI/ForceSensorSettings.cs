using HAL.Spatial;
using HAL.Units.Mass;
using System;
using System.Collections.Generic;
using System.Text;

namespace HAL.Documentation.ATI
{
    public class ForceSensorSettings
    {
        public ForceSensorSettings() { }
        public ForceSensorSettings(MatrixFrame frame, MatrixFrame sensorCoordinateSytem, Mass sensorMass, MatrixFrame sensorCenterOfMass)
        {
            Frame = frame;
            SensorCoordinateSytem = sensorCenterOfMass;
            SensorMass = sensorMass;
            SensorCenterOfMass = sensorCenterOfMass;
        }

        public MatrixFrame Frame { get; set; }
        public MatrixFrame SensorCoordinateSytem { get; set; }
        public Mass SensorMass { get; set; }
        public MatrixFrame SensorCenterOfMass { get; set; }
    }
}
