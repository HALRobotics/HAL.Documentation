using HAL.Reflection.Attributes;

namespace HAL.Documentation.Base.Monitoring
{
    public partial class RecordMarker
    {
        /// <summary> Get <see cref="RecordMarker"/> properties. </summary>
        /// <param name="record">Object to get properties from.</param>
        /// <param name="timeStamp">Time stamp.</param>
        /// <param name="alias">Marker name.</param>
        [Function("{02D4B108-C3C7-4E16-9C3C-7D4C27777D2D}", "Get Properties", "GetProperties", "Get RecordMarker Properties.", 0)]
        [FunctionSet("{C8A10170-9E77-4DBC-A481-49D111766713}", "RecordMarker Properties", "RecordMarker", "Get RecordMarker Properties.", 2)]
        [FunctionSuite(HAL.Documentation.Base.Monitoring.FonctionSuite.GetProperties)]
        public static void GetProperties(IRecord record, out int timeStamp, out string alias)
        {
            timeStamp = (int)record.TimeStamp;
            alias = record is RecordMarker recordMarker ? recordMarker.Identity.Alias : "";
        }
    }
}