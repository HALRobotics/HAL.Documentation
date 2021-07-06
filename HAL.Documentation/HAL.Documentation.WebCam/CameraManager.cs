using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HAL.Control;
using HAL.Control.Subsystems;
using HAL.Control.Subsystems.Communication;
using HAL.Documentation.KaplaPlusCamera.Providers;
using HAL.Documentation.KaplaPlusCamera.Providers.Sources;
using HAL.ImageAnalysis.Features;
using HAL.ImageAnalysis.Features.Events;
using HAL.ImageAnalysis.Features.Providers;
using HAL.ImageAnalysis.Implementation.Features;
using HAL.ImageAnalysis.Implementation.Features.Providers;
using HAL.Spatial;
using HAL.Tasks;
using HAL.Units.Length;

namespace HAL.Documentation.KaplaPlusCamera
{
    /// <summary> <see cref="CameraManager"/> event triggered when <see cref="IFeature"/> are received.</summary>
    public class CameraEventArg : ControllerState
    {
        /// <summary> Sensor event containing data as <see cref="IFeature"/>. </summary>
        /// <param name="features"></param>
        public CameraEventArg(Controller controller, List<IFeature> features) : base(controller, false, false, false) { Features = features; }

        /// <summary> <see cref="DistanceSensorManager"/>'s data. </summary>
        public List<IFeature> Features { get; }
    }

    /// <summary> Base <see cref="CameraManager"/> implementation using a <see cref="IImageFeatureProvider"/> like a <see cref="MultiMarkerProvider"/>. </summary>
    public class CameraManager : IStateReceivingSubsystem
    {

        #region Constructors
        
       

        public CameraManager(CameraManager clonee) : this(clonee.Provider) { Threshold = clonee.Threshold.Clone();   }

        /// <summary> Create a <see cref="CameraManager"/> using a <see cref="MultiMarkerProvider"/> as default <see cref="IImageFeatureProvider"/> if none is provided. </summary>
        /// <param name="threshold">Threshold used to filter position.</param>
        /// <param name="indexCamera">Camera index.</param>
        public CameraManager( Length threshold, int indexCamera = 0) 
        {
            // Hard-coded declaration of extrinsic camera parameters.
            var cameraPos = new Vector3D(0.29329, -0.0387, 1.19567);
            var cameraQuatValues = new[] { 0.040118, -0.705797, 0.706052, -0.041614 };
            var cameraQuat = new HAL.Numerics.Quaternion(cameraQuatValues);
            var cameraCalibratedFrame = new MatrixFrame(cameraPos, new RotationMatrix(cameraQuat));

            Provider = new MultiMarkerProvider(new ImageSourceBuilder(indexCamera), null, null, 0.01, cameraCalibratedFrame);
            Threshold = threshold;
            MarkersPositions = new Dictionary<Marker, Vector3D>();
        }

        /// <summary> Create a <see cref="CameraManager"/> using any <see cref="IImageFeatureProvider"/>. </summary>
        /// <param name="provider">Set <see cref="IImageFeatureProvider"/> for this camera.</param>
        public CameraManager( IImageFeatureProvider provider) 
        {
            Provider = provider;
        }

        
        #endregion

        #region Properties
        
        private CameraEventArg State { get; set; }
        ///<inheritdoc/>
        public IController Controller { get; set; }
        private bool FilterChanges { get; set; }
        private IImageFeatureProvider Provider { get; set; }
        private CancellationTokenSource Cancel { get; set; }
        private Dictionary<Marker, Vector3D> MarkersPositions { get; set; }
        /// <summary>Last updated <see cref="Marker"/>.</summary>
        public List<Marker> Markers => MarkersPositions.Keys.ToList();
        private Length Threshold { get; set; }

        public bool IsRunning { get; private set; }
        #endregion

        #region Methods
        
        ///<inheritdoc/>
        public IControllerSubsystem Clone() => new CameraManager(this);

        /// <summary> Start <see cref="CameraManager"/> processes. </summary>
        public Task Start()
        {
            // initialize camera and feature providers
            Cancel = new CancellationTokenSource();
            var tasks = Task.Run(async () => { await Provider.Initialize(); }, Cancel.Token);

            // wait complete initialization.
            Task.WaitAll(tasks);

            // start provider.
            Provider.Start();
            IsRunning = true;
            return Task.CompletedTask;
        }


        /// <summary> Stop <see cref="CameraManager"/> processes. </summary>
        public void Stop(bool closeConnection = false) //todo add close connection
        {
            // unsubscribe any event
            Unsubscribe();

            // stop provider
            Provider.Stop();
            

            IsRunning = false;
        }

        private void OnFeatureGrabbed(IFeatureProvider provider, IFeatureGrabbed grabbedFeature)
        {
            if (UpdateFeatures()) StateChanged?.Invoke(new CameraEventArg((Controller) Controller, provider.Features.Values.ToList()),State);
        }

        private void Subscribe()
        {
            Provider.OnFeatureGrabbed -= OnFeatureGrabbed;
            Provider.OnFeatureGrabbed += OnFeatureGrabbed;
        }

        private void Unsubscribe()
        {
            Provider.OnFeatureGrabbed -= OnFeatureGrabbed;
        }

        /// <summary> Get any <see cref="IFeature"/> like <see cref="Marker"/> detected by a <see cref="IImageFeatureProvider"/>. </summary>
        /// <returns>Detected <see cref="IFeature"/> list.</returns>
        public List<IFeature> GetFeatures() => Provider.GrabFeatures(out _);

        private bool UpdateFeatures()
        {
            bool changed = !FilterChanges;
            foreach (var feature in GetFeatures().OfType<Marker>())
            {
                var key = feature.MarkerProperty.Identity;
                if (!MarkersPositions.ContainsKey(feature))
                {
                    MarkersPositions[feature] = feature.Position;
                    changed = true;
                    continue;
                }

                if (MarkersPositions[feature].DistanceTo(feature.Position).GreaterThan(Threshold))
                {
                    MarkersPositions[feature] = feature.Position;
                    changed = true;
                }
            }
            return changed;
        }


        ///// <summary>Custom <see cref="CameraManager"/> event handler.</summary>
        ///// <param name="sender"></param>
        ///// <param name="eventArg"></param>
        //public delegate void CameraEventHandler(CameraManager sender, CameraEventArg eventArg);

        ///// <summary> Raised if <see cref="CameraManager"/> state is updated. </summary>
        //public event CameraEventHandler StateChanged;

        ///<inheritdoc/>
       public event IStateReceivingSubsystem.StateChangedHandler? StateChanged;


        ///<inheritdoc/>
        public void Dispose()
        {
            Provider.Stop();
            Cancel?.Dispose();
        }

        /// <summary>Stream <see cref="IImageFeatureProvider"/>'s detected features such as <see cref="Marker"/>.</summary>
        public void StreamFeatures(bool filterChanges, Length threshold = null)
        {
            FilterChanges = filterChanges;
            Threshold = threshold ?? Threshold;
            Subscribe();
        }

        public async Task<IControllerState> GetStateAsync(CancellationToken? cancel = null, BackgroundProgress progress = null)
        {
            if (UpdateFeatures())
            {
                var state = new CameraEventArg((Controller)Controller, Provider.Features.Values.ToList());
                StateChanged?.Invoke(state, State);
                State = state;
            }
            return State;
        }

        public bool TrySetNetworkIdentity(string hint)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}