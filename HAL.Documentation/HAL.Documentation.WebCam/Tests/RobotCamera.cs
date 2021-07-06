using ARCSO.PartMeasurement.Sensors;
using HAL;
using HAL.ABB.Control.Subsystems.RobotWebServices;
using HAL.ConsoleHelper.Runtime;
using HAL.ConsoleHelper.Utilities;
using HAL.Documentation.KaplaPlusCamera;
using HAL.Documentation.KaplaPlusCamera.Helpers;
using HAL.ENPC.ImageAnalysis.Features;
using HAL.ImageAnalysis.Implementation.Features;
using HAL.Spatial;
using HAL.Units.Length;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ARCSO.PartMeasurement.Tests
{
    public static class RobotCamera
    {
        
        private static RobotWebServicesManager _rws;
        public static async Task Run(string ipAdress = "127.0.0.1", int indexCamera = 0)
        {
            // add robot web services subsystem
            var controller = new HAL.ABB.Control.IRC5();
            _rws = Helpers.AddRwsManager(controller, ipAdress);


            // start camera
            var camera = new Camera((mm)20, indexCamera);
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
                camera.StateUpdated += OnStateUpdated;
                camera.StreamFeatures(true, (mm)100);
                Console.ReadLine();
            }
            camera.Stop();
        }

        private static void OnStateUpdated(Camera sender, CameraEventArg eventArg)
        {
            var messages = eventArg.Features.SelectMany(f => f is Marker marker ?
                new[] { $"{marker.Identity.Alias}", $"{marker.Position}", $"{marker.Rotation}" } : new string[] { }).ToArray();
            Logger.Log(messages);
        }
    }
}