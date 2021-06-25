using HAL.Control;
using HAL.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HAL.Objects.Mechanisms;
using HAL.Procedures.Actions;
using HAL.Motion.Settings;
using HAL.Motion;
using HAL.Units.Angle;
using HAL.Units.Length;
using Action = HAL.Procedures.Actions.Action;
using HAL.Procedures;
using HAL.Units.Time;
using HAL.Spatial;
using System.Reflection;
using HAL.ABB.Control;
using HAL.Control.Subsystems.Procedures;
using System.Threading;
using HAL.Documentation.Base;

namespace HAL.Documentation.SimpleApplication
{
    public class FunctioningCellFromSerialization
    {
        public static Logger InfoLogger;
        
        /// Deserialise a cell, 
        /// Create and assign a procedure,
        /// Export the procedure to a local folder or ulpoad it to a phsycial controller.
        public static async Task Run()
        {
            ///Create a new client and set the required assemblies. Mandatory step. 
            var client =  new Client(ClientBootSettings.Minimal,
            Assembly.GetAssembly(typeof(ABBController))
            //Assembly.GetAssembly(typeof(IProcedureExportingSubsystem)),
            //Assembly.GetAssembly(typeof(ILoadingCapableSubsystem)),
            );
           
            await client.StartAsync();

            InfoLogger = new Logger();

            #region Session and Cell

            ///Deserialize a session and the cell components.
            var session = Serialization.Helpers.DeserializeSession(@"C:\Users\ThomasDelaplanche\SerializedDocuments\SessionTestABB.hal", true);
            var controller = session.ControlGroup.Controllers.OfType<RobotController>().First();
            var mechanism = controller.Controlled.OfType<Mechanism>().First();
            //var mechanism = controller.Controlled.OfType<Mechanism>().Where(m=>m.Identity.Alias =="Robot");
            //var robot = mechanism.SubMechanisms.FirstOrDefault();
            #endregion

            #region Procedure

            ///Settings 

            ///To genreate a specific setting for each joint.
            var individualJointSpeeds = (new double[] { 10, 20, 10, 20, 10, 20 });
            var individualJointAccelerations = (new double[] { 10, 20, 10, 20, 10, 20 });

            ///To generate all the value at once.  
            var generalJointSpeeds = Enumerable.Repeat((double)1000, mechanism.ActiveJoints.Count).ToArray();
            var generalJointAccelerations = Enumerable.Repeat(new Angle((deg)2000).ToDouble(), mechanism.ActiveJoints.Count).ToArray();

            var approachJointSettings = new MotionSettings(
                        MotionSpace.Joint, new SpeedSettings(new JointSpeeds(mechanism.ActiveJoints, generalJointSpeeds)),
                        new AccelerationSettings(new JointAccelerations(mechanism.ActiveJoints, individualJointAccelerations)),
                        new BlendSettings(new Length((mm)1.0), new Angle((deg)1.0), new Length((mm)1)));

            /// Define the diferent actions to be performed.
            var actions = new List<Action>()
            {
                ///Motion with empty target and default settings
                new MotionAction(mechanism, new Target(MatrixFrame.Identity), DefaultSettings.Get<MotionSettings>(),"MinimalMotion"),
                ///Joint motion
                new MotionAction (mechanism,new Target(new JointPositions(new Angle[]{(deg)0, (deg)0, (deg)0, (deg)0, (deg)0, (deg)0 })), approachJointSettings," approachJoint"),
                /// Delay the exection
                new WaitTimeAction((s)3, "Pause"),
                /// Custom action :  write the identifier alias as a new line. Can be used to call a procedure predefined in the phsyscial controller.
                new ActionSet(new Identifier("DO_01"))
            };
            var procedure = new Procedure("BasicActions", actions);

            /// Assign the new procedure to the mechanism.
            controller.Assign(mechanism, procedure);

            #endregion

            #region Solving and export

            ///Subscribe to solving events. The logger will display the appropriate alerts when the events are triggered. 
            session.ControlGroup.Solver.SolvingCompleted += OnSolvingCompleted;
            session.ControlGroup.Solver.SolvingStarted += OnSolvingStarted;
            
            ///Start the solving. An unsolved procedure cannot be exported nor uploaded.
            session.ControlGroup.Solver.StartSolution();
            await controller.TryExportAsync(@"C:\Users\ThomasDelaplanche\SerializedDocuments", Linguistics.Export.DeclarationMode.Inline, cancel: CancellationToken.None); ///set the folder where you want to export.

            ///
            var canUpload = controller.SubsystemManager.TryGetSubsystem<ILoadingCapableSubsystem>(out var uploader);
            if (canUpload)
            {
                uploader.TrySetNetworkIdentity("192.168.0.103"); /// Set the real controller Ip adress.
                await controller.TryExportAndUploadAsync(Linguistics.Export.DeclarationMode.Inline, false, cancel: CancellationToken.None);
            }

            #endregion
        }



        #region Events  
        
        /// Actions to performs when the events are triggered 

        private static void OnSolvingStarted(object sender, EventArgs e)
        {
            var logged = InfoLogger.Log(Logger.SolvingStarted());
        }

        private static void OnSolvingCompleted(object sender, EventArgs e)
        {
            var logged = InfoLogger.Log(Logger.SolvingCompleted());
        } 

        #endregion
    }
}
