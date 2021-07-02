using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HAL.Communications;
using HAL.Control.Subsystems.Communication;
using HAL.Control.Subsystems.Signals;
using HAL.Documentation.Base.Monitoring;
using HAL.Helpers;
using HAL.Tasks;
using HAL.Units.Electrical;
using HAL.Units.Time;
using Controller = HAL.Control.Controller;
using Monitor = HAL.Documentation.Base.Monitoring.Monitor;

namespace HAL.Documentation.KaplaPlusCamera.Calibration
{
    /// <summary> Base implementation for controller calibration. </summary>
    public abstract class ControllerCalibration<TCalibrationData> : Identified where TCalibrationData : ICalibrationData
    {
        protected ControllerCalibration(string alias, Controller primaryController, List<Controller> secondaryControllers, string folderPath) : base(alias)
        {
            PrimaryController = primaryController;
            SecondaryControllers = secondaryControllers;
            FolderPath = folderPath;
        }

        protected ControllerCalibration(string alias, Controller primaryController, Controller secondaryController, string folderPath) : this(alias, primaryController, new List<Controller> { secondaryController }, folderPath) { }
       


        protected ControllerCalibration(string alias, List<Controller> secondaryControllers, Time tick, string folderPath) : base(alias)
        {
            Tick = tick;
            PrimaryController = null;
            SecondaryControllers = secondaryControllers;
            FolderPath = folderPath;
        }


        #region Properties
        /// <summary>  </summary>
        public Time Tick { get; set; }

        /// <summary>  </summary>
        internal string FolderPath { get; set; }

        private ICalibrationData CalibrationData { get; set; }

        /// <summary>  </summary>
        public Monitor Monitor { get; private set; }

        /// <summary> Controller of the sensor which is being calibrated. </summary>
        public Controller PrimaryController { get; set; }

        /// <summary> Controller of the sensor which is being calibrated. </summary>
        public List<Controller> SecondaryControllers { get; set; }

        private BackgroundProgress Progress { get; set; }
        private CancellationToken Cancel { get; set; }


        #region Triggers
        private ElectricSignal ConnectTrigger { get; set; }
        private ElectricSignal RecordTrigger { get; set; }
        #endregion
        #endregion

        #region Methods

        #region Trigger events management

        internal void InitializeTriggers(ElectricSignal connectTrigger, ElectricSignal recordTrigger)
        {
            UnsubscribeToConnectTrigger();
            UnsubscribeToRecordTrigger();

            ConnectTrigger = connectTrigger;
            RecordTrigger = recordTrigger;

            SubscribeToConnectTriggers();
            SubscribeToRecordTriggers();
        }

        #region Connect signal trigger
        internal void SubscribeToConnectTriggers()
        {
            UnsubscribeToConnectTrigger();
            ConnectTrigger.StateChanged -= OnConnectTriggerStateChanged;
            ConnectTrigger.StateChanged += OnConnectTriggerStateChanged; ;
        }

        private void UnsubscribeToConnectTrigger()
        {
            if (!(ConnectTrigger is null)) ConnectTrigger.StateChanged -= OnConnectTriggerStateChanged;
        }

        private void OnConnectTriggerStateChanged(object sender, EventArgs e)
        {
            if (!(sender is ElectricSignal socket) || socket.Quantization != SignalQuantization.Digital) return;

            if (socket.State.Value.GreaterThan((V)12))
            {
                Monitor.AddMarker("Connect");
                Monitor.Start(true, true, Cancel);
            }
            else
            {
                Monitor.AddMarker("Disconnect");
                Monitor.Stop(false, false);
                _acquisitionTokenSource.Cancel();
            }
        }

        #endregion

        #region Record signal trigger
        internal void SubscribeToRecordTriggers()
        {
            UnsubscribeToRecordTrigger();
            RecordTrigger.StateChanged -= OnRecordTriggerStateChanged;
            RecordTrigger.StateChanged += OnRecordTriggerStateChanged;
        }

        private void UnsubscribeToRecordTrigger()
        {
            if (!(RecordTrigger is null)) RecordTrigger.StateChanged -= OnRecordTriggerStateChanged;
        }

        private void OnRecordTriggerStateChanged(object sender, EventArgs e)
        {
            if (!(sender is ElectricSignal socket) || socket.Quantization != SignalQuantization.Digital) return;

            if (socket.State.Value.GreaterThan((V)12))
            {
                Monitor.AddMarker("Resume");
                Monitor.Resume(true, false);
            }
            else
            {
                Monitor.AddMarker("Pause");
                Monitor.Pause();
            }
        }

        #endregion

        private CancellationTokenSource _acquisitionTokenSource;
        #endregion

        /// <summary>  </summary>
        internal void InitializeMonitor()
        {
            Monitor?.Stop();

            IStateReceivingSubsystem stateReceiver = null;
            bool? initialized = PrimaryController?.SubsystemManager.TryGetSubsystem(out stateReceiver);

            var stateReceivers = SecondaryControllers.Select(c => c.SubsystemManager.TryGetSubsystem(out IStateReceivingSubsystem subsystem) ? subsystem : null).ToList();

            Monitor = initialized is null
                ? new Monitor(Identity.Alias, stateReceivers, Tick, FolderPath)
                : new Monitor(Identity.Alias, stateReceivers, stateReceiver, FolderPath);
            var stop=0;
        }

        /// <summary> </summary>
        public virtual async Task CalibrateAsync(ICalibrationSettings settings, bool save, BackgroundProgress progress, CancellationToken cancel)
        {
            Cancel = cancel;
            Progress = progress;
            Progress.ResizeWindow(4);

            _acquisitionTokenSource = new CancellationTokenSource();
            await Initialize(settings);
            Progress.ReportProgress(1);

            await _acquisitionTokenSource.Token.WhenCanceled();
            Progress.ReportProgress(1);

            Console.WriteLine("Solving");
            await Solve();
            Progress.ReportProgress(1);

            if (save) Save();
            Progress.ReportProgress(1);

            Disconnect();
        }

        private void Disconnect()
        {
            UnsubscribeToConnectTrigger();
            UnsubscribeToRecordTrigger();
            MonitorSignals(false);
        }

        private void MonitorSignals(bool monitor)
        {
            
            List<Controller> controllers = new List<Controller>();
            controllers.AddRange(SecondaryControllers);
            controllers.AddIfUnique(PrimaryController);
            foreach (var controller in controllers)
            {
                if (controller.SubsystemManager.TryGetSubsystem(out IGettableSignalSubsystem subsystem))
                    subsystem.MonitorSignals = monitor;
            }
        }

        /// <summary> Ensure settings are valid. </summary>
        /// <param name="settings">Setting to use.</param>
        /// <returns>Completed task.</returns>
        internal virtual Task Initialize(ICalibrationSettings settings)
        {
            InitializeMonitor();
            InitializeTriggers(settings.ConnectTrigger, settings.RecordTrigger);
            Monitor.Start();
            Console.WriteLine("Monitor started");
            return Task.CompletedTask;
        }


        private async Task<bool> TryMapTriggers(List<Controller> controllers, ElectricSignal socket)
        {
            var succeded = true;
            Runtime.Client.Instance.ActiveSession.CommunicationGraph.TryGetInEdges(socket, out var edges);
            if (socket is ElectricSignal electrical)
            {

                foreach (var bus in edges)
                {
                    var controller = controllers.FirstOrDefault(c => bus.Source is Controller ctrl && c.Equals(ctrl));
                    if (!(controller is null) && controller.SubsystemManager.TryGetSubsystem(out ISignalSubsystem subsystem))
                    {
                        succeded = succeded && await subsystem.TryMapSignalAsync(electrical, Cancel);
                    }
                }
            }

            //if (socket is CommunicatingObject communicatingObject)
            //{
            //    throw new NotImplementedException();
            //}

            return succeded;
        }

        /// <summary>Use the recorded data to calculate the calibration. </summary>
        /// <returns>Completed task.</returns>
        protected abstract Task<TCalibrationData> Solve();

        /// <summary>  </summary>
        /// <returns></returns>
        internal void Save() => SerializationHelpers.SerializeJsonAndCompress(Identity.Alias, FolderPath, CalibrationData, "calibration");
    }
    #endregion
}