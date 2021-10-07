using HAL.Objects;
using HAL.OptiTrack.Control;
using HAL.OptiTrack.Control.Subsystems;
using HAL.OptiTrack.Data;
using HAL.Runtime;
using HAL.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HAL.Documentation.OptiTrack
{
    class SimpleDataAcquisition
    {
        static async Task Main(string[] args)
        {
            /// Start the client.
            var client = new Client(ClientBootSettings.Minimal);
            await client.StartAsync();

            /// Creates a Reference to be applied to the OptiTrackController. The matrix frame should be the transformation matrix between the OptiTrack world and the phyisical world. It has beeen computed during the calibration.
            /// A parent such as the robot can be assigned to the frame during the creation.
            var instance = new Reference();
            Reference.Create(ref instance, "calibration", MatrixFrame.Identity, out Reference calibration);

            /// Creates an OptiTrack controller. 
            OptiTrack = new OptiTrackController(null, null, calibration);
            var manager = new OptiTrackManager();
            OptiTrack.SubsystemManager.Add(manager);

            /// Changed the IP address of the data provider. 
            /// By default the IP is set on the loopback address of the computer. 
            /// If the data comes from the Motive software on the same computer, the loopback address can be used and this line is not required.
            manager.TrySetNetworkIdentity("192.100.100.100");

            /// Used to modify the OptitrackSettings. If default settings are used, this line is not required.
            manager.SetFilterSettings(new OptiTrackSettings());

            ///Start then stop the continuous reception of data and subscribe/unsubscribe to the state changed event. 
            await manager.Start();
            manager.StateChanged -= OnManagerStateChanged;
            manager.StateChanged += OnManagerStateChanged;
            await Task.Delay(5000);
            manager.StateChanged -= OnManagerStateChanged;
            manager.Stop();

            ///actualize and retrieve the controller's data for a single measurement.
            var currentState = (OptiTrackControllerState)await manager.GetStateAsync();

            /// Marker positions in the OptiTrack's World.
            var markerPositionInOptiTrackWorld = currentState.Markers.Select(m => m.Position).ToList();
            /// Marker positions in the Reference's World. Ex: the robot's world or the session's world.
            var markerPositionInReferenceWorld = currentState.Markers.Select(m => m.GetReferencedPosition(calibration).LocationInWorld(true).Position).ToList();

        }

        #region Properties

        /// OptiTrack Controller.
        public static OptiTrackController OptiTrack;
        #endregion

        ///This code will be executed when a new state is assigned to the controller.
        private static void OnManagerStateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is OptiTrackControllerState state)
            {
                /// Console display of the name of the detected bodies and the 
                var bodies = string.Join(", ", state.Bodies.Select(fB => fB.Alias));
                Console.WriteLine($"Frame processed with {bodies} visible and {state.Markers.Count} markers visible");

                /// Bodies frames in the Reference world.
                var bodiesOrientedPositions = state.Bodies.Select(fB => fB.GetReferencedPosition(OptiTrack.Reference).LocationInWorld(true));

            }
        }
    }
}
