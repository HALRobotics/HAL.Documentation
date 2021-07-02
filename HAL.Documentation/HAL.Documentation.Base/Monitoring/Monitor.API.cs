using System.Collections.Generic;
using HAL.Alerts;
using HAL.Control;
using HAL.Control.Subsystems.Communication;
using HAL.Reflection.Attributes;
using HAL.Simulation;

namespace HAL.Documentation.Base.Monitoring
{
    /// <summary> Base implementation for <see cref="Monitor"/> </summary>
    public partial class Monitor
    {
        /// <summary> Create a <see cref="Monitor"/>. </summary>
        /// <param name="instance">Existing instance.</param>
        /// <param name="alias">Alias.</param>
        /// <param name="controllers">Controllers.</param>
        /// <param name="primary">Primary controller used as trigger.</param>
        /// <param name="folderPath">Path acces to save recorded files.</param>
        /// <param name="monitor"><see cref="Monitor"/></param>
        [Function("{F1633E0D-BCF8-4393-8CF5-46C85A5E4701}", "Monitor", "Monitor", "Monitor controler states", 0)]
        [FunctionSuite("{9D6F8216-3FEE-412A-9FA5-E9BB3BF8C566}", "Monitor", "Monitor", "Monitor", 0)]
        [FunctionSubcategory(HAL.Documentation.Base.Monitoring.FonctionSubCategory.Monitoring, "Monitoring", "Monitoring", "Monitoring", 0)]
        [FunctionCategory(FonctionCategory.Application, "Applications", "Applications", "Applications", 602)]
        public static void Create(
            [Hidden, Cached] ref Monitor instance,
            [Default("Monitor")] string alias,
            List<IController> controllers,
            IController primary,
            string folderPath,
            out Monitor monitor)
        {
            if (!(instance is null) && instance.IsMonitoring)
            {
               // if (!(instance.ExecutionControl is null)) instance.ExecutionControl.Execution = ExecutionMode.Stop;
                instance.Stop();
            }
            Create(alias, controllers, primary, folderPath, out monitor);
            instance = monitor;
        }


        /// <summary> Create a <see cref="Monitor"/>. </summary>
        /// <param name="alias">Alias.</param>
        /// <param name="controllers">Controllers.</param>
        /// <param name="primary">Primary controller used as trigger.</param>
        /// <param name="folderPath">Path acces to save recorded files.</param>
        /// <param name="monitor"><see cref="Monitor"/></param>
        public static void Create(string alias, List<IController> controllers, IController primary, string folderPath, out Monitor monitor)
        {
            var receivers = ExtractSubsystems(controllers);
            IStateReceivingSubsystem trigger = null;
            primary?.SubsystemManager.TryGetSubsystem(out trigger);
            monitor = new Monitor(alias, receivers, trigger, folderPath);
        }

        private static List<IStateReceivingSubsystem> ExtractSubsystems(List<IController> controllers)
        {
            var stateReceivers = new List<IStateReceivingSubsystem>();
            foreach (var controller in controllers)
            {
                controller.SubsystemManager.TryGetSubsystem(out IStateReceivingSubsystem subsystem);
                stateReceivers.Add(subsystem);
            }
            return stateReceivers;
        }

        /// <summary> Control state monitor. </summary>
        /// <param name="monitor">Monitor to control.</param>
        /// <param name="executionControl">Control.</param>
        /// <param name="record">Whether it should record received states.</param>
        [Function("{E8D3FA87-5FC6-4F68-B6B0-5BA5C972EBC8}", "Execute", "Execute", "Control state monitor", 0)]
        [FunctionSuite("{00E88BF9-3A34-4BBD-8CE5-7ED2AD4B8EE4}", "Execute", "Execute", "Control state monito", 1)]
        [FunctionSubcategory(HAL.Documentation.Base.Monitoring.FonctionSubCategory.Monitoring)]
        public static void Execute(Monitor monitor, bool executionControl, [Default(false)] bool record)
        {
            monitor.IsRecording = record;
            monitor.ExecutionControl = executionControl;
        }

        /// <summary>Add Record Marker. </summary>
        /// <param name="monitor">Monitor to control.</param>
        /// <param name="alias">Alias</param>
        /// <param name="add">Whether to add a marker.</param>
        [Function("{F11ABDB7-2CAA-4CC9-872B-4BBC8262C000}", "Add Marker", "AddMarker", "Add Record Marker", 0)]
        [FunctionSuite("{1C60782B-E068-441D-8FE8-19F05C56D3EC}", "Add Marker", "AddMarker", "Add Record Marker", 2)]
        [FunctionSubcategory(HAL.Documentation.Base.Monitoring.FonctionSubCategory.Monitoring)]
        public static void AddMarker(Monitor monitor, string alias, bool add)
        {
            if (add) monitor.AddMarker(alias);
        }

        /// <summary> Save recorded data if any. </summary>
        /// <param name="monitor"><see cref="Monitor"/>.</param>
        /// <param name="save">Save</param>
        /// <param name="clear">Clear data after saving them.</param>
        [Function("{2E5D3A19-7FC3-4B57-9422-04282337E550}", "Save Data", "SaveData", "Save recorded data", 0)]
        [FunctionSuite("{2CCE7D5A-2F0C-4EDA-8C12-50BCAD931588}", "Save Data", "SaveData", "Save recorded data", 3)]
        [FunctionSubcategory(HAL.Documentation.Base.Monitoring.FonctionSubCategory.Monitoring)]
        [Background("save")]
        public static void Save(Monitor monitor, bool save, [Default(true)] bool clear)
        {
            if (monitor.IsMonitoring)
            {
                // todo : add/log alert instead of throwing.
                //throw new Alert("Monitoring", AlertLevel.Error, "Monitoring Enable", "Monitoring enable, stop monitoring before saving data.");
            }
            monitor.Pause(save);
            monitor.Resume(monitor.IsRecording, clear);

        }

        /// <summary>Get <see cref="Monitor"/> properties </summary>
        /// <param name="monitor">Object to get properties from.</param>
        /// <param name="recorder"><see cref="Recorder"/></param>
        [Function("{F99FAF15-C380-49B0-9516-DBCDA8D8B0D2}", "Get Properties", "GetProperties", "Get Properties of a Monitor.", 0)]
        [FunctionSet("{07BBE2F3-29DD-4625-AA72-9B2CE1088EAB}", " Monitor Properties", "Monitor", "Get Monitor Properties", 0)]
        [FunctionSuite(HAL.Documentation.Base.Monitoring.FonctionSuite.GetProperties, "Monitoring Properties", "MonitoringProperties", "Get the properties of a Monitor, Recorder or Record.", 4)]
        [FunctionSubcategory(HAL.Documentation.Base.Monitoring.FonctionSubCategory.Monitoring)]
        public static void GetProperties(Monitor monitor, out Recorder recorder)
        {
            recorder = monitor?.Recorder;
        }
    }
}