using HAL.Control;
using HAL.Documentation.KaplaPlusCamera.Calibration;
using HAL.Documentation.KaplaPlusCamera.Helpers;
using HAL.Documentation.KaplaPlusCamera.Providers.Tests;
using HAL.ImageAnalysis.Implementation.Features;
using HAL.Objects.Mechanisms;
using HAL.Runtime;
using HAL.Spatial;
using HAL.Units.Length;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HAL.Documentation.KaplaPlusCamera
{
    class Program
    {
      //  static ConsoleLogger Logger = new ConsoleLogger();

        private static async Task Main()
        {
            await RunCalibration.Run();
            //CameraOnly.Run(0);
        //    ///Create a new client and set the required assemblies. Mandatory step. 
        //    var client = new Client(ClientBootSettings.Minimal

        //    //Assembly.GetAssembly(typeof(IProcedureExportingSubsystem)),
        //    //Assembly.GetAssembly(typeof(ILoadingCapableSubsystem)),
        //    );

        //    await client.StartAsync();

        //    // InfoLogger = new Logger();

        //    #region Session and Cell

        //    ///Deserialize a session and the cell components.
        //    var session = Serialization.Helpers.DeserializeSession(@"C:\Users\ThomasDelaplanche\SerializedDocuments\SessionTestABB.hal", true);
        //    var controller = session.ControlGroup.Controllers.OfType<RobotController>().First();
        //    var mechanism = controller.Controlled.OfType<Mechanism>().First();
        //    #endregion

        //    var camera = new Camera((mm)20, indexCamera);
        //    camera.Start();

        //    var getMarker = true;

        //    while (getMarker)
        //    {
        //        getMarker = Prompt.PromptConfirmation("Get marker position");
        //        if (!getMarker) continue;

        //        var features = camera.GetFeatures();
        //        foreach (var feature in features)
        //        {
        //            var position = feature is Marker marker ? marker.Position : (Vector3D?)null;
        //            Logger.Log(new[] { position.ToString() });
        //        }
        //    }

        //    if (Prompt.PromptConfirmation("Stream marker position"))
        //    {
        //        camera.StateUpdated += OnStateUpdated;
        //        camera.StreamFeatures(true, (mm)100);
        //        Console.ReadLine();
        //    }
        //    camera.Stop();
        //}

        //private static void OnStateUpdated(Camera sender, CameraEventArg eventArg)
        //{
        //    var messages = eventArg.Features.SelectMany(f => f is Marker marker ?
        //        new[] { $"{marker.Identity.Alias}", $"{marker.Position}", $"{marker.Rotation}" } : new string[] { }).ToArray();
        //    Logger.Log(messages);
        }
    }
}
