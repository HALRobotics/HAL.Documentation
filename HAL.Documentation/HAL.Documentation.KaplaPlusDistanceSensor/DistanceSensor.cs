using HAL;
using HAL.Numerics;
using HAL.Units;
using HAL.Units.Absolute;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HAL.Documentation.KaplaPlusDistanceSensor
{
    public class DistanceSensorEventArg
    {
        /// <summary> Sensor event containing data as <see cref="string"/>. </summary>
        /// <param name="data"></param>
        public DistanceSensorEventArg(string data) { Data = data; }

        /// <summary> <see cref="DistanceSensorManager"/>'s data. </summary>
        public string Data { get; }
    }

    public class DistanceSensor : Identified
    {
        public DistanceSensor(string alias, int index) : base(alias)
        {
            Index = index;
        }

        public int Index { get; private set; }
        public int Value { get; set; }
        //public abs Value => Range.Clamp(RawValue).Value;
        //private abs RawValue { get; set; }
        //private AbsoluteInterval Range { get; set; }
        //public void Calibrate(abs t0, abs t1) => Range = new AbsoluteInterval(t0, t1);

    }

    /// <summary> Base implementation of a <see cref="DistanceSensorManager"/> communicating through <see cref="SerialPort"/>. </summary>
    public class DistanceSensorManager : IDisposable
    {
        #region Fields
        private static SerialPort _serialPort;
        #endregion

        #region Constructors
        /// <summary> Create a new <see cref="DistanceSensorManager"/>. </summary>
        /// <param name="portName">Set <see cref="SerialPort"/> port name.</param>
        /// <param name="baudrate">Set <see cref="SerialPort"/> baudrate.</param>
        public DistanceSensorManager(int sensorNumber, string portName, int baudrate = 9600)
        {
            PortName = portName;
            Baudrate = baudrate;
            DistanceSensors = new DistanceSensor[sensorNumber];
            for (int i = 0; i < DistanceSensors.Length; i++)
            {
                DistanceSensors[i] = new DistanceSensor($"Sensor-{i}", i);
            }
        }
        #endregion

        #region Properties

        private bool IsInitialized { get; set; }
        private string PortName { get; }
        private int Baudrate { get; }
        private CancellationTokenSource Cancel { get; set; }
        private string Message { get; set; }
        public DistanceSensor[] DistanceSensors { get; set; }
        #endregion

        #region Methods
        private bool Initialize()
        {
            IsInitialized = false;
           _serialPort= _serialPort ??  new SerialPort { PortName = PortName, BaudRate = Baudrate };

            try
            {
                _serialPort.Open();
                IsInitialized = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($@"Sensor is not running. Verify if the sensor is connected on {PortName} or change port name.");
            }

            return IsInitialized;
        }

        /// <summary> Start <see cref="DistanceSensorManager"/> data reception. </summary>
        public void Start()
        {
            Stop();
            if (!Initialize()) return;
            Cancel = new CancellationTokenSource();
            Task.Run(async () => { await ReadSerialPort(Cancel.Token); }, Cancel.Token);
        }

        /// <summary> Stop <see cref="DistanceSensorManager"/> data reception. </summary>
        public void Stop()
        {
            IsInitialized = false;
            Cancel?.Cancel();
            Cancel = null;
            _serialPort?.Close();
        }

        private async Task ReadSerialPort(CancellationToken cancel)
        {
            while (IsInitialized || !cancel.IsCancellationRequested)
            {
                await ReadMessage();
            }
        }

        /// <summary>Custom <see cref="DistanceSensorManager"/> event handler.</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SensorEventHandler(DistanceSensorManager sender, DistanceSensorEventArg e);

        /// <summary> Raised if <see cref="DistanceSensorManager"/> state is updated. </summary>
        public event SensorEventHandler StateUpdated;

        private Task ReadMessage()
        {
            Message = TryReadMessage();
            var s = Message.TrimEnd('\r', '\n');
            InterpretMessage(s);
            StateUpdated?.Invoke(this, new DistanceSensorEventArg(Message));
            return Task.CompletedTask;
        }

        /// <summary> Raised if <see cref="DistanceSensorManager"/> message was "Stop". </summary>
        public event EventHandler StopReceived;

        /// <summary> Interpretation message logic. </summary>
        /// <param name="message"></param>
        protected virtual void InterpretMessage(string message)
        {
            if (message == "STOP") StopReceived?.Invoke(this, new EventArgs());
            else InterpretMessageData(message);
        }

        protected virtual void InterpretMessageData(string message)
        {
            var datas = message.Split(';').Select(s => int.TryParse(s, out var value) ? value : -1).ToArray();
            for (int i = 0; i < datas.Length; i++)
            {
                
                DistanceSensors[i].Value = datas[i];
            }
        }

        //public async Task CalibrateSensors(int timeWindows)
        //{
        //    RecordedRawValues = new Dictionary<int, List<int>>();
        //    for (int i = 0; i < DistanceSensor.Length; i++)
        //    {
        //        Console.WriteLine($"Calibrating sensor number {i}.");
        //        if (HAL.ConsoleHelper.Runtime.ConsoleClient.PromptConfirmation("Start calibration?"))
        //        {
        //            RecordedRawValues[i] = new List<int>();
        //            await CalibrateSensor(index, timeWindows);
        //        }

        //    }

        //    foreach (var records in RecordedRawValues)
        //    {
        //    }

        //}

        //private async Task CalibrateSensor(int index, int timeWindows)
        //{

        //    Start();
        //    StateUpdated -= OnStateUpdate;
        //    StateUpdated += OnStateUpdate;

        //    await Task.Delay(timeWindows);
        //    StateUpdated -= OnStateUpdate;

        //}

        //private Dictionary<int, List<int>> RecordedRawValues { get; set; }
        //private void OnStateUpdate(DistanceSensorManager sender, DistanceSensorEventArg e)
        //{
        //    int[] array = InterpretMessageData(e.Data);
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        RecordedRawValues[i].Add(array[i]);
        //    }
        //}

        private string TryReadMessage()
        {
            try
            {
                return _serialPort.ReadLine();
            }
            catch (TimeoutException) { }

            return "";
        }

        ///<inheritdoc/>
        public void Dispose()
        {
            Stop();
            Cancel?.Dispose();
        }
        #endregion
    }
};