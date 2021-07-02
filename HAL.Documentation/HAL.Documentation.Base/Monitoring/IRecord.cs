using System.Runtime.Serialization;

namespace HAL.Documentation.Base.Monitoring
{
    /// <summary> Qualifies a dated record. </summary>
    public interface IRecord
    {
        /// <summary> Time stamp. </summary>
        public long TimeStamp { get; }
    }
}