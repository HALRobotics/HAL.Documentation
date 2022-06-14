using HAL.Alerts;
using HAL.Control;
using HAL.Control.Subsystems.Communication;
using HAL.Objects;
using HAL.Objects.Mechanisms;
using HAL.Objects.Mechanisms.Processes;
using HAL.Units.Time;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HAL.Documentation.Base.Monitoring
{
    /// <summary> Based implementation of sensor monitoring. </summary>
    public partial class Monitor : Identified
    {
        /// <summary> Create a new sensor monitoring instance with recording on controller state changed. </summary>
        /// <param name="alias">Alias.</param>
        /// <param name="stateReceivers">Monitored controller with <see cref="IStateReceivingSubsystem"/> connected to sensors.</param>
        /// <param name="trigger">Trigger <see cref="IStateReceivingSubsystem"/> used as trigger for monitoring all subsystems.</param>
        /// <param name="folderPath">path used to save recorded states.</param>

        public Monitor(string alias, List<IStateReceivingSubsystem> stateReceivers, IStateReceivingSubsystem trigger, string folderPath) : base(alias)
        {
            Tick = null;
            Trigger = trigger;
            if (!(Trigger is null) && !stateReceivers.Contains(Trigger)) stateReceivers.Add(Trigger);
            StateReceivers = stateReceivers;

            Recorder = new Recorder(alias, folderPath);
        }

        /// <summary> Create a new sensor monitoring instance with recording on a constant time based trigger.</summary>
        /// <param name="alias">Alias</param>
        /// <param name="stateReceivers">Monitored controller with <see cref="IStateReceivingSubsystem"/> connected to sensors.</param>
        /// <param name="tick">Time period when controller's state are monitored.</param>
        /// <param name="folderPath">path used to save recorded states.</param>
        public Monitor(string alias, IList<IStateReceivingSubsystem> stateReceivers, Time tick, string folderPath) : base(alias)
        {
            Tick = tick;
            // todo [IMPROVEMENT]: ensure controller list contains primary controller if necessary. 
            Trigger = null;
            if (!(Trigger is null) && !stateReceivers.Contains(Trigger)) stateReceivers.Add(Trigger);
            StateReceivers = stateReceivers;
            Recorder = new Recorder(alias, folderPath);
        }

        #region Properties
        /// <summary> <see cref="Monitoring.Recorder"/> where <see cref="StateRecord"/> are stored.</summary>
        public Recorder Recorder { get; set; }

        /// <summary> Last recorded states. </summary>
        public StateRecord CurrentRecord { get; private set; }

        /// <summary> Whether is recording. </summary>
        public bool IsRecording { get; set; }

        /// <summary> Whether is monitoring. </summary>
        public bool IsMonitoring { get; private set; }

        /// <summary> Logger </summary>
        public ILogger Logger { get; set; }

        private IStateReceivingSubsystem Trigger { get; }
        private IList<IStateReceivingSubsystem> StateReceivers { get; }
        private CancellationTokenSource CancellationSource { get; set; }
        private CancellationToken Cancel { get; set; }
        private Stopwatch Timer { get; set; }
        private Time Tick { get; }

        /// <summary>Controller to which this manager is subscribed and which will control its simulation.</summary>
        public bool ExecutionControl
        {
            get => _executionControl;
            set
            {
                if (_executionControl == value) return;
                if (_executionControl.Equals(true)) Start(clear: true);
                else if (_executionControl.Equals(false)) Stop(true, false);

            }
        }
        private bool _executionControl;
        #endregion

        #region Methods
        //todo: add Control of the monitor by a new class type ControlGroup
        #region ExecutionControl

        ///// <summary>Replaces the visitor execution control with a specified one.</summary>
        ///// <param name="control">New control to link this visitor with.</param>
        //public void SwapExecutionControl(MonitorGroupExecutor control)
        //{
        //    UnlinkExecutionControl();
        //    _executionControl = control;
        //    LinkExecutionControl();
        //}

        /// <summary>Unlinks this visitor from its current simulation control.</summary>
        //public void UnlinkExecutionControl()
        //{
        //    if (_executionControl is null) return;

        //    _executionControl.ExecutionStarted -= OnExecutionStarted;
        //    _executionControl.ExecutionStopped -= OnExecutionStopped;
        //    _executionControl.ExecutionReset -= OnExecutionReset;
        //    _executionControl = null;
        //}

        // private void OnExecutionStarted(object sender, EventArgs e)
        //=> Start(clear: true);

        // private void OnExecutionStopped(object sender, EventArgs e)
        //=> Stop(true, false);

        // private void OnExecutionReset(object sender, EventArgs e)
        // {
        //     var isRecording = IsRecording;
        //     if (IsMonitoring)
        //     {
        //         Task.Factory.StartNew(() =>
        //         {
        //             Stop(false, false);
        //             Start(isRecording, true);
        //         });
        //     }
        //     else Start(isRecording, true);
        // }

        //public void LinkExecutionControl()
        //{
        //    if (_executionControl is null) return;

        //    _executionControl.ExecutionStarted += OnExecutionStarted;
        //    _executionControl.ExecutionStopped += OnExecutionStopped;
        //    _executionControl.ExecutionReset += OnExecutionReset;
        //}
        #endregion
        #endregion

        #region Events
        protected internal virtual void SubscribeTrigger()
        {
            if (Trigger is null)
            {
                StateUpdated -= UpdateCurrentStatesOnTriggerEvent;
                StateUpdated += UpdateCurrentStatesOnTriggerEvent;
            }
            else
            {
                Trigger.StateChanged -= UpdateCurrentStatesOnTriggerEvent;
                Trigger.StateChanged += UpdateCurrentStatesOnTriggerEvent;
            }
        }

        protected internal virtual void UnsubscribeTrigger()
        {
            StateUpdated -= UpdateCurrentStatesOnTriggerEvent;
            if (!(Trigger is null)) Trigger.StateChanged -= UpdateCurrentStatesOnTriggerEvent;
        }


        /// <summary> Raised if controller states are changed.</summary>
        public event EventHandler StateChanged;

        private event Action<IState, IState> StateUpdated;

        private async void UpdateCurrentStatesOnTriggerEvent(IState current, IState previous)
        {
            if (Timer is null) return;
            IState[] states = new IState[StateReceivers.Count];
            for (var index = 0; index < StateReceivers.Count; index++)
            {
                var receiver = StateReceivers[index];
                states[index] = await receiver.GetStateAsync(Cancel);
            }
            CurrentRecord = new StateRecord(Timer.ElapsedMilliseconds, states);
            if (IsRecording) Recorder.Add(CurrentRecord);
            StateChanged?.Invoke(this, null);
        }

        private async Task StartMonitoringAsync(bool isMonitoring)
        {
            IsMonitoring = isMonitoring;
            var delay = (int)Math.Ceiling(((ms)Tick.Value).Value);
            if (delay < 1) return;

            while (IsMonitoring || Cancel.IsCancellationRequested)
            {
                StateUpdated?.Invoke(null, null);
                await Task.Delay(delay, Cancel);
            }
            IsMonitoring = false;
        }

        #endregion

        private bool TryRunAllSubsystems(CancellationToken cancel)
        {
            Console.WriteLine("start");
            Task.WaitAll(StateReceivers.Select(s => s.Start()).ToArray(), cancel);
            Console.WriteLine("all started");
            return StateReceivers.All(s => s.IsRunning);
        }

        #region Execution
        /// <summary> Start monitoring. </summary>
        /// <param name="token">Cancellation token. If null, an internal <see cref="CancellationToken"/> will be used.</param>
        /// <param name="record">Whether the data should be recorded.</param>
        /// <param name="clear">Whether the <see cref="Recorder"/> should be cleared</param>
        public void Start(bool record = false, bool clear = false, CancellationToken? token = null)
        {
            // clear previous recording session
            if (clear) Recorder.Clear();
            IsRecording = record;
            Timer?.Stop();
            CancellationSource?.Cancel();

            Cancel = token ?? CancellationToken.None;
            // ensure subsystem are running. Wait all that they start.
            if (!TryRunAllSubsystems(Cancel))
            {
                Logger?.Log(StartAllSubsystemsAlert);
                return;
            }
            Console.WriteLine("All Systems started");
            // reset and start tick.
            Timer = Stopwatch.StartNew();

            // subscribe and raised events.
            SubscribeTrigger();
            Logger?.Log(StartAlert);

            // run loop if no trigger provided.
            if (Trigger is null)
            {
                CancellationSource = new CancellationTokenSource();
                Task.Run(async () => await StartMonitoringAsync(true), Cancel.CanBeCanceled ? Cancel : CancellationSource.Token);
            }
            else IsMonitoring = true;
        }

        /// <summary> Pause monitoring. </summary>
        /// <param name="save">Whether the recorded states should be serialized and saved.</param>
        public void Pause(bool save = false)
        {
            if (!IsMonitoring) { Logger?.Log(StartAllSubsystemsAlert); return; }
            UnsubscribeTrigger();
            if (save) Recorder.Save(false);
            Logger?.Log(PauseAlert);
        }

        /// <summary> Resume monitoring. </summary>
        /// <param name="record">Whether the data should be recorded.</param>
        /// <param name="clear">Whether the <see cref="Recorder"/> should be cleared</param>
        public void Resume(bool record = false, bool clear = true)
        {
            if (!IsMonitoring) { Logger?.Log(StartAllSubsystemsAlert); return; }
            if (clear) Recorder.Clear();
            IsRecording = record;
            SubscribeTrigger();
            Logger?.Log(ResumeAlert);
        }

        /// <summary> Stop monitoring. </summary>
        /// <param name="save">Whether the recorded states should be serialized and saved.</param>
        /// <param name="clear">Whether the <see cref="Recorder"/> should be cleared</param>
        public void Stop(bool save = false, bool clear = true)
        {
            Timer?.Stop();
            Timer = null;
            UnsubscribeTrigger();
            CancellationSource?.Cancel();
            IsMonitoring = false;
            IsRecording = false;
            if (save) Recorder.Save(clear);
            if (clear) Recorder.Clear();
            Logger?.Log(StopAlert);
        }
        #endregion

        /// <summary> Add time line marker. </summary>
        /// <param name="alias">Marker name.</param>
        public void AddMarker(string alias) => Recorder.Add(new RecordMarker(alias, Timer?.ElapsedMilliseconds ?? 0));

        #region Helpers

        /// <summary>Get all same state type contained in this recorder.</summary>
        /// <typeparam name="TState">Type of state to get.</typeparam>
        /// <param name="states">Found states if any.</param>
        public void GetAllStates<TState>(out List<TState> states) where TState : IState
        {
            states = new List<TState>();
            foreach (var record in Recorder.Records.OfType<StateRecord>().ToList())
            {
                TryGetStates(record, out List<TState> s);
                states.AddRange(s);
            }
        }

        /// <summary>Try get a specific type of state in current states.</summary>
        /// <typeparam name="TState">Type of state to get.</typeparam>
        /// <param name="states">Found states if any.</param>
        /// <returns>Whether if any state where found.</returns>
        public bool TryGetCurrentStates<TState>(out List<TState> states) where TState : IState => TryGetStates(CurrentRecord, out states);

        /// <summary>Try get a specific type of state in current states.</summary>
        /// <typeparam name="TState">Type of state to get.</typeparam>
        /// <param name="stateRecord"><see cref="StateRecord"/> containing states.</param>
        /// <param name="states">Found states if any.</param>
        /// <returns>Whether if any state where found.</returns>
        /// <returns></returns>
        public bool TryGetStates<TState>(StateRecord stateRecord, out List<TState> states) where TState : IState
        {
            var allStates = new List<IState>();
            foreach (var state in stateRecord.States)
            {
                switch (state)
                {
                    case TState typedState:
                        allStates.Add(typedState);
                        break;
                    case ControllerState controllerState:
                        var mechanicalStates = controllerState.MechanicalState.ToList();
                        var toolStates = mechanicalStates.Select(m => m.Tool).ToList();
                        allStates.Add(controllerState);
                        allStates.AddRange(mechanicalStates);
                        allStates.AddRange(toolStates);
                        break;
                    case MechanismState mechanicalState:
                        allStates.Add(mechanicalState);
                        allStates.Add(mechanicalState.Tool);
                        break;
                    case ToolState toolState:
                        allStates.Add(toolState);
                        break;
                }
            }
            states = allStates.OfType<TState>().ToList();
            return states.Count > 0;
        }

        #endregion

        #region Alerts
        /// <summary>Default alert for IsRunning all subsystem.</summary>
        protected Alert StartAllSubsystemsAlert => new CommunicationAlert(AlertLevel.Info,
            "Start Subsystems.", "Impossible to start all subsystems. Impossible to start the monitoring.");

        /// <summary>Default alert for starting to monitor.</summary>
        protected Alert StartAlert => new CommunicationAlert(AlertLevel.Info,
            "Start monitoring.", $"Is monitoring: {IsMonitoring} Is recording: {IsRecording}.");

        /// <summary>Default alert for pausing to monitor.</summary>
        protected Alert PauseAlert => new CommunicationAlert(AlertLevel.Info,
            "Pause monitoring.", $"Is monitoring: {IsMonitoring} Is recording: {IsRecording}.");

        /// <summary>Default alert for resuming to monitor.</summary>
        protected Alert ResumeAlert => new CommunicationAlert(AlertLevel.Info,
            "Resume monitoring.", $"Is monitoring: {IsMonitoring} Is recording: {IsRecording}.");

        /// <summary>Default alert for stopping to monitor.</summary>
        protected Alert StopAlert => new CommunicationAlert(AlertLevel.Info,
            "Stop monitoring.", $"Is monitoring: {IsMonitoring} Is recording: {IsRecording}.");

        #endregion

        public string HumanReadableStates()
        {
            var str = $"Last record : {CurrentRecord}{Environment.NewLine}";
            return str;
        }
    }
}