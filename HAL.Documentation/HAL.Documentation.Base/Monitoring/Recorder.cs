using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HAL.Alerts;
using HAL.Helpers;
using HAL.Objects;
using HAL.Units.Time;

namespace HAL.Documentation.Base.Monitoring
{
    /// <summary> Base implementation a <see cref="IState"/> recorder with common time stamps.</summary>
    [DataContract]
    public partial class Recorder : Identified
    {
        #region Constructors

        /// <summary> Create a new <see cref="Recorder"/>. </summary>
        /// <param name="alias">Alias.</param>
        /// <param name="folderPath">Folder path where recorded states should be saved.</param>
        /// <param name="logger">Logger.</param>
        public Recorder(string alias, string folderPath, ILogger logger = null) : base(alias)
        {
            Logger = logger;
            Records = new List<IRecord>();
            FolderPath = folderPath;
        }
        #endregion

        #region Properties
        /// <summary> Logger. </summary>
        public ILogger Logger { get; }

        /// <summary> Recorded states.</summary>
        [DataMember]
        public List<IRecord> Records { get; private set; }

        /// <summary>Folder path used to save recorded states. </summary>
        public string FolderPath { get; set; }
        #endregion

        #region Methods
        /// <summary> Add <see cref="IRecord"/> to this <see cref="IRecord"/>.</summary>
        /// <param name="record">Recorded states to add.</param>
        public void Add(IRecord record) => Records.Add(record);

        /// <summary> Clear all saved and current <see cref="IRecord"/>. </summary>
        public void Clear()
        {
            Records.Clear();
            Logger?.Log(OnClearAlert());
        }


        /// <summary> Save <see cref="Records"/> in a serialized and compressed json file. </summary>
        /// <param name="clear">Whether <see cref="Records"/> and <see cref="CurrentRecord"/> should be cleared.</param>
        /// <param name="folderPath">Folder path.</param>
        /// <param name="fileName">File name.</param>
        /// <returns></returns>
        public bool Save(bool clear, string folderPath = null, string fileName = null)
        {
            folderPath ??= FolderPath;
            if (string.IsNullOrEmpty(FolderPath)) return false;

            //var records = new List<IRecord>();
            //records.AddRange(Records);

            fileName ??= $"{Identity.Alias}{DateTime.Now:yyyyMMddTHHmmss}";
            var saved = SerializationHelpers.SerializeJsonAndCompress(fileName, folderPath, this, "record");
            Logger?.Log(Records.Count > 0 && saved
                ? SaveSucceededAlert(fileName)
                : SaveFailureAlert());
            if (clear) Clear();
            return true;
        }

        /// <summary> Create subset of <see cref="IRecord"/> from a <see cref="Recorder"/> between <see cref="RecordMarker"/> and create subsets of <see cref="IRecord"/>. </summary>
        /// <returns>A list of <see cref="Recorder"/>.</returns>
        public List<Recorder> GroupRecords()
        {
            List<Recorder> subRecorders = new List<Recorder>();

            var index = 0;
            var previousIndex = 0;
            long previousTimeStamp = 0;
            foreach (var record in Records)
            {
                if (record is RecordMarker recordMarker)
                {
                    var recorder = new Recorder($"{Identity.Alias}_{index}_Start_{previousTimeStamp}_End_{recordMarker.TimeStamp}", FolderPath);
                    recorder.Records.AddRange(Records.GetRange(previousIndex, index - previousIndex));
                    subRecorders.Add(recorder);
                    previousIndex = index;
                    previousTimeStamp = recordMarker.TimeStamp;
                }
                index++;
            }

            return subRecorders;
        }

        #endregion

        #region Alerts
        /// <summary>Default alert for stopping to monitor.</summary>
        protected Alert SaveFailureAlert() => new CommunicationAlert(AlertLevel.Error,
            $"Save Failed.", $"Recorded data contains state : {Records.Any()}");

        /// <summary>Default alert for stopping to monitor.</summary>
        protected Alert SaveSucceededAlert(string fileName) => new CommunicationAlert(AlertLevel.Info,
            $"Saved recorded states.", $"{fileName} saved in {FolderPath}.{Environment.NewLine}" + ToString());
        /// <summary>Default alert for stopping to monitor.</summary>
        protected Alert OnClearAlert() => new CommunicationAlert(AlertLevel.Info, "Recorder Cleared", $"This {Identity} was cleared");
        #endregion

        ///<inheritdoc/>
        public override string ToString()
        {
            var str = base.ToString() + Environment.NewLine;
            str += $"Recorded states count : {Records.Count}.{Environment.NewLine}" +
                   $"Recorded time frame : {(ms)((int)Records.Last().TimeStamp - Records.First().TimeStamp)}";
            return str;
        }
    }
}