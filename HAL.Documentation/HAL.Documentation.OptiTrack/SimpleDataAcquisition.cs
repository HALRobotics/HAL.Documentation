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

            ///By default the IP is set on the loopback address of the computer and no Ipaddress is required.
            var optitrackCell = new OptitrackCell(calibration);
            optitrackCell.RunReception(1000);
             
            ///actualize and retrieve the controller's data for a single measurement.
            var currentState = (OptiTrackControllerState)await optitrackCell.Manager.GetStateAsync();

            /// Marker positions in the OptiTrack's World.
            var markerPositionInOptiTrackWorld = currentState.Markers.Select(m => m.Position).ToList();
            /// Marker positions in the Reference's World. Ex: the robot's world or the session's world.
            var markerPositionInReferenceWorld = currentState.Markers.Select(m => m.GetReferencedPosition(calibration).LocationInWorld(true).Position).ToList();

        }
        
    }
}
