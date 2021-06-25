using HAL.Communications;
using HAL.Procedures;

namespace HAL.Documentation.KaplaPlusCamera.Calibration
{
    public interface ICalibrationSettings : IIdentifiable
    {
        /// <summary> Connect (24V) or disconnect (0V) to monitoring. </summary>
        ElectricSignal ConnectTrigger { get; }
        /// <summary> Start (24V) or stop (0V) recording. </summary>
        ElectricSignal RecordTrigger { get; }

    }
}