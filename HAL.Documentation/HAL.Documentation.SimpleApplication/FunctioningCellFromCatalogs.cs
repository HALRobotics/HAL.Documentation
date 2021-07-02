using HAL.ABB.Control;
using HAL.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HAL.Objects.Mechanisms;
using HAL.Catalogs.Items;
using HAL.Control;
using HAL.Procedures;
using HAL.Procedures.Actions;
using HAL.Motion.Settings;
using Action = HAL.Procedures.Actions.Action;
using HAL.Motion;
using HAL.Units.Time;
using HAL.Units.Angle;
using HAL.Units.Length;
using System.Threading;
using HAL.Units.Speed;
using HAL.Control.Subsystems.Procedures;
using System.Net;
using HAL.ABB.Control.Subsystems.Procedures;
using HAL.Control.Subsystems.Communication;
using HAL.Spatial;
using System.IO;
using HAL.Objects.Mechanisms.Processes;
using HAL.Units.Mass;

namespace HAL.Documentation.SimpleApplication
{
    public class FunctioningCellFromCatalogs
    {
        public static async Task Run()
        {
            ///Create a new client. Mandatory step. 
            await new Client(ClientBootSettings.Minimal, new Assembly[] {
                Assembly.GetAssembly(typeof(IStateReceivingSubsystem)),
                Assembly.GetAssembly(typeof(ABBController)),
                Assembly.GetAssembly(typeof(IProcedureExportingSubsystem)),
                Assembly.GetAssembly(typeof(ILoadingCapableSubsystem)) }).StartAsync();
            var client = Client.Instance;

            #region Session and Cell

            ///The Session containing every object
            var session = client.ActiveSession;

            ///Retrieve a controller and a mechanism from the catalogs. Add them to the control group. The control group manages the solving of the procedures.
            var controller = (RobotController)await client.Catalogs.Controllers.RetrieveAsync("c391cd57-4094-4b53-9bc4-fc3816805551");
            var robot = (Mechanism)await client.Catalogs.Mechanisms.RetrieveAsync("69ae7bf9-fc98-47e8-9661-9818ebc3767c");
            Tool tool = null;
            Tool.Create(ref tool, "Gripper", MatrixFrame.Identity, new List<Mesh>(), (kg)1, MatrixFrame.Identity, out tool);
            robot.AddSubMechanism(tool, null, out _);
            controller.AddControlledObject(robot);
            session.ControlGroup.Add(controller);

            #region Language

            // manage resources files and copy in application folder
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(path);
            var directoryName = Path.GetDirectoryName(path);
            var languageFileName = Path.Combine(directory, "Language.lang");
            Console.WriteLine($@"'{Path.GetFileName(languageFileName)}' file created at ('{directoryName}')");
            var rapid6ExportingSubsystem = new Rapid6ExportingSubsystem();
            controller.SubsystemManager.Add(rapid6ExportingSubsystem);

            #endregion

            #endregion

            #region Procedure

            ///Settings 

            ///To genreate a specific setting for each joint.
            var individualJointSpeeds = (new double[] { 10, 20, 10, 20, 10, 20 });
            var individualJointAccelerations = (new double[] { 10, 20, 10, 20, 10, 20 });

            ///To generate all the value at once.  
            var generalJointSpeeds = Enumerable.Repeat((double)1000, robot.ActiveJoints.Count).ToArray();
            var generalJointAccelerations = Enumerable.Repeat(new Angle((deg)2000).ToDouble(), robot.ActiveJoints.Count).ToArray();


            var approachJointSettings = new MotionSettings(
                        MotionSpace.Joint, new SpeedSettings(new JointSpeeds(robot.ActiveJoints, generalJointSpeeds)),
                        new AccelerationSettings(new JointAccelerations(robot.ActiveJoints, individualJointAccelerations)),
                        new BlendSettings(new Length((mm)1.0), new Angle((deg)1.0), new Length((mm)1)));

            var actions = new List<Action>()
            {
                ///Motion with empty target and default settings
                new MotionAction(robot, new Target(MatrixFrame.Identity), DefaultSettings.Get<MotionSettings>(),"MinimalMotion"),
                ///Joint motion
                new MotionAction (robot,new Target(new JointPositions(new Angle[]{(deg)0, (deg)0, (deg)0, (deg)0, (deg)0, (deg)0 })), approachJointSettings," approachJoint"),
                new WaitTimeAction((s)3, "Pause"),
                new ActionSet(new Identifier("CustomAction"))
            };
            var procedure = new Procedure("BasicActions", actions);
            controller.Assign(robot, procedure);

            #endregion

            #region Solving and export

            Console.WriteLine("Solving");
            session.ControlGroup.Solver.StartSolution();
            Console.WriteLine("Procedures Solved");
            await controller.TryExportAsync(@"C:\Users\ThomasDelaplanche\SerializedDocuments", Linguistics.Export.DeclarationMode.Inline, cancel: CancellationToken.None);
            await controller.TryExportAndUploadAsync(Linguistics.Export.DeclarationMode.Inline, false, cancel: CancellationToken.None);

            #endregion



        }

    }
}
