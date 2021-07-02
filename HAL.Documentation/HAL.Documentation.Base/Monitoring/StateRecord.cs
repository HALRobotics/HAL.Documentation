using System;
using System.Linq;
using System.Runtime.Serialization;
using HAL.Objects;

namespace HAL.Documentation.Base.Monitoring
{

    /// <summary> Base implementation of a list of <see cref="IState"/> and their common timestamp.</summary>
    [DataContract]
    public partial class StateRecord : IEquatable<StateRecord>, IRecord
    {

        /// <summary> Create a new record at a specified time of a list of an array <see cref="IState"/>. </summary>
        /// <param name="timestamps">Current time stamps.</param>
        /// <param name="states">States sharing the same time stamp to record.</param>
        public StateRecord(long timestamps, IState[] states)
        {
            TimeStamp = timestamps;
            States = states;
        }

        /// <summary> Time stamp. </summary>
        [DataMember]
        public long TimeStamp { get; }

        /// <summary> States. </summary>
        [DataMember]
        public IState[] States { get; }

        ///<inheritdoc/>
        public bool Equals(StateRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TimeStamp == other.TimeStamp;
        }

        ///<inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StateRecord)obj);
        }

        ///<inheritdoc/>
        public override int GetHashCode() => TimeStamp.GetHashCode();


        public override string ToString()
        {
            var str = $"Time stamps : {TimeStamp}" + Environment.NewLine;
            return States.Aggregate(str, (current, state) => current + ($"{state.Date} : {state.GetType()}." + Environment.NewLine));
        }
    }
}