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
    public class OptitrackCell
    {
        public OptitrackCell(Reference reference = null, String IpAddress = null, OptiTrackSettings settings = null )
        {
            /// Creates an OptiTrack controller. 
            Controller = new OptiTrackController(null, null, reference);
            Manager = new OptiTrackManager();
            Controller.SubsystemManager.Add(Manager);
            if (IpAddress != null) Manager.TrySetNetworkIdentity(IpAddress);
            if (settings != null) Manager.SetFilterSettings(settings);
            
        }
        public OptiTrackController Controller { get; set; }
        public OptiTrackManager Manager { get; set; }

        /// <summary> Start then stop the continuous reception of data and subscribe/unsubscribe to the state changed event. </summary>
        /// <param name="executionTime">Total time of the acquisition, if null the acquisition will continue indefinitely.</param>
        public async void RunReception(int? executionTime = null)
        {
             
            await Manager.Start();
            Manager.StateChanged -= OnManagerStateChanged;
            Manager.StateChanged += OnManagerStateChanged;
            if (executionTime is not null)
            {
                await Task.Delay(executionTime.Value);
                Manager.StateChanged -= OnManagerStateChanged;
                Manager.Stop();
            }
        }


        ///This code will be executed when a new state is assigned to the controller.
        private static void OnManagerStateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is OptiTrackControllerState state)
            {
                /// Console display of the name of the detected bodies and the 
                var bodies = string.Join(", ", state.Bodies.Select(fB => fB.Alias));
                Console.WriteLine($"Frame processed with {bodies} visible and {state.Markers.Count} markers visible");

                /// Bodies frames in the Reference world.
                var bodiesOrientedPositions = state.Bodies.Select(fB => fB.GetReferencedPosition(((OptiTrackController)state.Source).Reference).LocationInWorld(true));

            }
        }

    }
}
