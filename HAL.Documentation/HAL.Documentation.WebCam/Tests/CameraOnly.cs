using HAL.Documentation.KaplaPlusCamera;
using HAL.Documentation.KaplaPlusCamera.Helpers;
using HAL.ImageAnalysis.Implementation.Features;
using HAL.Spatial;
using HAL.Units.Length;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HAL.Documentation.KaplaPlusCamera.Providers.Tests
{
    public static class CameraOnly
    {
        static ConsoleLogger Logger = new ConsoleLogger();

        public static async Task Run(int indexCamera = 0)
        {
            var camera = new CameraManager((mm)20, indexCamera);
            camera.Start();

            var getMarker = true;

            while (getMarker)
            {
                getMarker = Prompt.PromptConfirmation("Get marker position");
                if (!getMarker) continue;

                var features = camera.GetFeatures();
                foreach (var feature in features)
                {
                    var position = feature is Marker marker ? marker.Position : (Vector3D?)null;
                    Logger.Log(new[] { position.ToString() });
                }
            }

            if (Prompt.PromptConfirmation("Stream marker position"))
            {
                camera.StateChanged += OnStateChanged;
                camera.StreamFeatures(true, (mm)100);
                Console.ReadLine();
            }
            camera.Stop();
        }

        private static void OnStateChanged(Objects.IState current, Objects.IState previous)
        {
            if (current is CameraEventArg state)
            {
                var messages = state.Features.SelectMany(f => f is Marker marker ?
                   new[] { $"{marker.Identity.Alias}", $"{marker.Position}", $"{marker.Rotation}" } : new string[] { }).ToArray();
                Logger.Log(messages);
            }
            
        }

        
    }
}