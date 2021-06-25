using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARCSO.PartMeasurement.Processes;
using ARCSO.PartMeasurement.Sensors;
using HAL;
using HAL.ABB.Control.Subsystems.EGM;
using HAL.ABB.Control.Subsystems.RobotWebServices;
using HAL.Alerts;
using HAL.Communications;
using HAL.ConsoleHelper.Cell;
using HAL.ConsoleHelper.Runtime;
using HAL.ConsoleHelper.Utilities;
using HAL.Documentation.KaplaPlusCamera;
using HAL.ENPC.ImageAnalysis.Features;
using HAL.ImageAnalysis.Implementation.Features;
using HAL.Motion;
using HAL.Motion.Planning;
using HAL.Objects.Mechanisms;
using HAL.Procedures;
using HAL.Runtime;
using HAL.Spatial;
using HAL.Tasks;
using HAL.Units.Angle;
using HAL.Units.Electrical;
using HAL.Units.Length;

namespace ARCSO.PartMeasurement.Tests
{
    public class Application
    {
        #region Fields
        private static ConsoleLogger Logger = new ConsoleLogger();
        private static Vector3D _position;
        private static ElectricSignal do_01 = new ElectricSignal(new Identifier("DO_01"), SignalQuantization.Digital, Direction.Out, 0, 24);
        private static ElectricSignal do_02 = new ElectricSignal(new Identifier("DO_02"), SignalQuantization.Digital, Direction.Out, 0, 24);
        private static RobotWebServicesManager rws;
        #endregion

        public static async Task Run(string distanceSensorCOM)
        {
            // take marker position
            await TakeMarkerPosition(1);

            // start robot
            // TODO : find values

            var initPositions = new[] { -20d, 28d, -23d, 0d, 62d, 0d }.Select(v => ((rad)(deg)v).Value).ToArray();
            await StartRobot(_position, initPositions);

        }

        private static Task TakeMarkerPosition(int indexCamera = 1)
        {
            var camera = new Camera((mm)20, indexCamera);
            var position = Vector3D.Default;
            camera.Start();

            var getMarker = true;
            while (getMarker)
            {
                getMarker = ConsoleClient.PromptConfirmation("Get marker position");
                if (!getMarker) continue;

                var features = camera.GetFeatures();
                foreach (var feature in features)
                {
                    position = feature is Marker marker ? marker.Position : Vector3D.Default;
                    Logger.Log(new[] { position.ToString() });
                }

                getMarker = !ConsoleClient.PromptConfirmation("Save marker position");
            }
            _position = position;

            return Task.CompletedTask;
        }

        private static void OnStopReceived(object sender, EventArgs e)
        {
            //do_02.State = new ElectricTension(24);
            //rws.TrySetSignalValueAsync(do_02, 24);
            rws.TryStopExecutionAsync(true);
            //ConsoleClient.Logger.AddLine = true;
            //ConsoleClient.Logger.Log(new Alert("Stop", AlertLevel.Warning, "Stop", "Robot will now stop."));
        }

        private static void OnStateUpdated(Sensors.DistanceSensorManager sender, DistanceSensorEventArg e)
        {
            //ConsoleClient.Logger.Log(new []{ $"{e.Data}"});
        }

        private static async Task StartRobot(Vector3D position, double[] initPositions)
        {
            #region Initialization
            // manage resources files and copy in application folder
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(path);

            var directoryName = Path.GetDirectoryName(path);
            var languageFileName = Path.Combine(directory, "Language.lang");
            File.WriteAllBytes(languageFileName, Properties.Resources.ABB_RAPID_6_0);
            Console.WriteLine($@"'{Path.GetFileName(languageFileName)}' file created at ('{directoryName}')");

            var sessionFileName = Path.Combine(directory, "Session.hal");
            File.WriteAllBytes(sessionFileName, Properties.Resources.ARCSO_ABB_TOOL_01);
            Console.WriteLine($@"'{Path.GetFileName(sessionFileName)}' file created at ('{directoryName}')");
            #endregion

            //import a session
            Session.Load(directoryName, Path.GetFileNameWithoutExtension(sessionFileName));

            // select a controller
            var controller = Session.SelectController();


            // TODO
            // Loading bloque à 1%
            // Vérifier la position donnée par le matrix frame (l'origine a l'air pourtant ok ? Redémarrer robotstudio ? Réessayer avec les anciens settings ?
            // ligne 135 new Vector3D(position.Y / 50, position.X / 50, 0.01d)
            // En retirant la division par 50 sur position Y le programme solve mais beaucoup plus lentement.
            // Déclarer le frame dans la classe MMP pour réorienter les targets à un endroit atteignable.
            // Essayer de rentrer une target hardcodée atteignable.

            #region debugging stuff
            //var debugX = 0.377412; // (mm)
            //var debugY = 0.011259; // (mm)
            //var debugZ = 0.385627; // (mm)
            //var debugposition = new Vector3D(debugX,debugY,debugZ);
            //double[] quaternionValues = {0, 0, 1, 0};
            //var quaternion = new HAL.Numerics.Quaternion(quaternionValues);
            //var debugQuaternionFrame = new QuaternionFrame(debugposition,quaternion);
            //var debugMatrixFrame = new MatrixFrame(debugQuaternionFrame);

            //var debugframe = new MatrixFrame(new Vector3D(0.37, 0.01, 0.01d), new RotationMatrix(-Vector3D.ZAxis));


            #endregion

            #region Create process procedure
            // select main actor (mechanism)
            var actor = controller.Controlled.OfType<Mechanism>().First();
            var homePosition = HAL.ConsoleHelper.Procedures.MotionActions.JointMove("HomePosition",
                new JointPositions(actor.ActiveJoints, initPositions),
                HAL.ConsoleHelper.Motion.Joints.JointSpeed(actor, 4, 300));
            var process = new Process();

            var frame = new MatrixFrame(new Vector3D(position.X, position.Y, 0.01d), new RotationMatrix(-Vector3D.ZAxis));

            var procedure = process.CreatePartMeasurement("PartMeasurementProcess", do_01, do_02, frame);
            var combined = new Procedure(new List<Procedure> { homePosition, procedure }, new Identifier("PartMeasurement"));
            combined.UpdateSimulationGraph();
            controller.ProcedureManager.Clear();
            controller.ProcedureManager.Add(combined, actor);
            #endregion

            // add robot web services subsystem
            var rws = Helpers.AddRwsManager(controller, "127.0.0.1");
            var egm = Helpers.AddEgmSubsystem(controller, "127.0.0.1", 6510);

            // solve procedures
            controller?.SolveProcedures(SolvingFidelity.Coarse);

            // print motion alerts
            //controller?.PrintAlerts();

            // upload to ABB remote controller
            // (in a virtual session in robot studio, need robot studio opened and to use the provided station)
            var progress = new BackgroundProgress();
            await controller.Upload(controller.ProcedureManager.Procedures.ToList(), "127.0.0.1", false, null, progress);

            // map signals and start monitoring.
            await rws.TryMapSignalAsync(do_02);
            await rws.StartMonitoringSignals();
            await rws.TryStartExecutionAsync(true);
        }
    }
}
