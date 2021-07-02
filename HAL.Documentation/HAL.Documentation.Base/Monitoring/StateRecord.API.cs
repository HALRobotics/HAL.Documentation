using System.Collections.Generic;
using System.Linq;
using HAL.Objects;
using HAL.Reflection.Attributes;

namespace HAL.Documentation.Base.Monitoring
{
    public partial class StateRecord
    {
        /// <summary> Get <see cref="StateRecord"/> properties. </summary>
        /// <param name="record">Object to get properties from.</param>
        /// <param name="timeStamp">Time stamp.</param>
        /// <param name="states">Recorded States.</param>
        [Function("{BC3F1859-220C-4819-A8E2-A6E06FE7A984}", "Get Properties", "GetProperties", "Get StateRecord Properties.", 0)]
        [FunctionSet("{21A03FE4-4227-4E3C-93DA-B5E12151961E}", "StateRecord Properties", "StateRecord", "Get StateRecord Properties.", 3)]
        [FunctionSuite(HAL.Documentation.Base.Monitoring.FonctionSuite.GetProperties)]
        public static void GetProperties(IRecord record, out int timeStamp, out List<IState> states)
        {
            timeStamp = (int)record.TimeStamp;
            states = record is StateRecord stateRecord ? stateRecord.States.ToList() : null;
        }
    }
}