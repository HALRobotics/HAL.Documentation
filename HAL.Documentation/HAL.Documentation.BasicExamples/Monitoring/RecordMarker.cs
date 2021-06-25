using System.Runtime.Serialization;
using HAL.Extensibility;

namespace HAL.Documentation.Base.Monitoring
{
    /// <summary> Base implementation for <see cref="RecordMarker"/>.</summary>
    [DataContract]
    public partial class RecordMarker : Identified, IRecord, IExtensible
    {
        /// <summary> Create a new record marker.</summary>
        /// <param name="alias">Alias.</param>
        /// <param name="timeStamps">Time stamp.</param>
        public RecordMarker(string alias, long timeStamps) : base(alias)
        {
            TimeStamp = timeStamps;
            IsExtended = false;
            ExtensionProperties = new PropertyManager(this);
        }

        /// <inheritdoc/>
        [DataMember]
        public long TimeStamp { get; }
        /// <inheritdoc/>
        public bool IsExtended { get; }
        /// <inheritdoc/>
        public PropertyManager ExtensionProperties { get; }
    }
}