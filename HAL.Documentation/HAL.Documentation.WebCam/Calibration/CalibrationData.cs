using System;
using System.Runtime.Serialization;
using HAL.Control;
using Newtonsoft.Json;

namespace HAL.Documentation.KaplaPlusCamera.Calibration
{
    [DataContract]
    public abstract class CalibrationData<TData> : SettingsBase, ICalibrationData where TData : new()
    {
        #region MyRegion
        protected CalibrationData(TData value) => Value = value;
        #endregion

        #region Properties
        ///<inheritdoc />
        public override bool IsValid { get; }

        /// <summary> Calibration data value. </summary>
        [DataMember]
        public TData Value { get; }

        ///<inheritdoc />
        object ICalibrationData.Value => Value;
        #endregion

    }

    public interface ICalibrationData : IIdentifiable
    {
        /// <summary> Calibration data value. </summary>
        object Value { get; }
    }
}