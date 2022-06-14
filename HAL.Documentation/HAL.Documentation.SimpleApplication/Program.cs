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
using HAL.Units.Speed;
using HAL.Alerts;

namespace HAL.Documentation.SimpleApplication
{
    public class Program
    {
        #region Fields

        public static Logger InfoLogger;

        #endregion

        // Deserializes a cell, 
        // Creates and assigns a procedure,
        // Exports the procedure to a local folder or upload it to a physical controller
        private static async Task Main(string[] args)
        {
            // Creates a new client and sets the required assemblies. Mandatory step 
            await new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(ABBController))).StartAsync();

            // Logger to display informations
            InfoLogger = new Logger();

            #region Session and Cell

            // Add the path there
            string sessionFile = null; 

            // Deserializes a session and the cell components  
            var serializedSession = sessionFile ?? @"C:\Users\User\SerializedDocuments\SessionTestABB.hal"; //Todo pointer to serialized session
            var session = Serialization.Helpers.DeserializeSession(serializedSession, true);
            var controller = session.ControlGroup.Controllers.OfType<RobotController>().First();
            var mechanism = controller.Controlled.OfType<Mechanism>().First();
                        
            #endregion

            #region Procedure

            // Diferent ways to configure motion settings 
            /// Get the default values for <see cref="MotionSettings"/>.
            var defaultMotionSettings = DefaultSettings.Get<MotionSettings>();

            // Generate custom settings from existing ones, by cloning to remove any reference to the original object 
            var customMotionSettings = (MotionSettings) defaultMotionSettings.Clone();
            
            /// Modify space settings. Motions can be interpreted in the cartesian space or joint space. It is specified by the Space property <see cref="MotionSpace"/> as follows
            var customCartesianSettings = (MotionSettings)defaultMotionSettings.Clone();
            customCartesianSettings.Space = MotionSpace.Cartesian;
            
            // In Cartesian space, the motions are performed by applying settings relative to the TCP (e.g. linear speed and angular speed of the TCP).
            // Here are some example on how to set or perform operations on the linear speed.
            customMotionSettings.SpeedSettings.PositionSpeed = (m_s)10;
            customMotionSettings.SpeedSettings.PositionSpeed += (m_s)1;
            customMotionSettings.SpeedSettings.PositionSpeed /= 2;

            // In joint space, the settings of each joint must be specified. It can be simpler to create settings from scratch.
            // Generate a specific setting for each joint. Example with joint speeds
            var individualJointSpeeds = (new double[] { 10, 10, 10, 20, 10, 20 });
            
            // Generate all the value at once. Example with joint accelerations 
            var generalJointAccelerations = Enumerable.Repeat(new Angle((deg)20).ToDouble(), mechanism.ActiveJoints.Count).ToArray();
            
            // Generate joint settings using the two variables specified before
            var customJointSettings = new MotionSettings(
                        MotionSpace.Joint, new SpeedSettings(new JointSpeeds(mechanism.ActiveJoints, individualJointSpeeds)),
                        new AccelerationSettings(new JointAccelerations(mechanism.ActiveJoints, generalJointAccelerations)),
                        new BlendSettings(new Length((mm)1.0), new Angle((deg)1.0), new Length((mm)1)));



            // Generate targets to reach
            var basicTarget = new Target(new MatrixFrame(new Vector3D(100, 100, 100), RotationMatrix.Identity.RotateAroundX(Math.PI))); ///target from a frame.
            Target.Transform(basicTarget, "", (mm)50, (mm)0, (mm)0, (deg)0, (deg)0, (deg)45, null, out var transformedTarget); /// target from a transformation of a frame.
            var jointPositions = new Target(new JointPositions(new Angle[] { (deg)0, (deg)0, (deg)0, (deg)0, (deg)0, (deg)0 }));/// target from joint position.



            // Generate actions to be performed
            var actions = new List<Action>()
            {
                // Motions from previously  and default settings
                new MotionAction(mechanism, basicTarget, defaultMotionSettings,"MinimalMotion"),
                
                // Motion from a transformed target and previously created Cartesian motion settings. 
                new MotionAction (mechanism, transformedTarget , customCartesianSettings, "cartesianMotion"),
                
                // Motion from a new joint position and the previously created joint motion settings.
                new MotionAction (mechanism,jointPositions, customJointSettings," approachJoint"),
                
                // Delay the execution
                new WaitTimeAction((s)3, "Pause"),
                
                // Custom action :  write the identifier alias as a new line. Can be used to call a procedure predefined in the physical controller.
                new ActionSet(new Identifier("DO_01"))
            };

            // Generate a procedure containing the different actions
            var procedure = new Procedure("BasicActions", actions);

            // Assign the new procedure to the mechanism.
            controller.Assign(mechanism, procedure);

            #endregion

            #region Solving and export

            // Subscribe to solving events. The logger will display the appropriate alerts when the events are triggered 
            session.ControlGroup.Solver.SolvingCompleted += OnSolvingCompleted;
            session.ControlGroup.Solver.SolvingStarted += OnSolvingStarted;

            // Start the solving. An unsolved procedure cannot be exported nor uploaded
            session.ControlGroup.Solver.StartSolution();
            await controller.TryExportAsync(@"C:\Users\ThomasDelaplanche\SerializedDocuments", Linguistics.Export.DeclarationMode.Inline, cancel: CancellationToken.None); ///todo: set pointer and argument

            // Select a subsystem able to export and upload code
            var canUpload = controller.SubsystemManager.TryGetSubsystem<ILoadingCapableSubsystem>(out var uploader);
            if (canUpload)
            {
                uploader.TrySetNetworkIdentity("192.168.0.103"); // Todo: argument, Set the real controller Ip address.
                await controller.TryExportAndUploadAsync(Linguistics.Export.DeclarationMode.Inline, false, cancel: CancellationToken.None);
            }

            #endregion
        }

        #region Events  

        // Actions to performs when the events are triggered 

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


    public class Logger : ILogger
    {
        public bool IsActive { get; set; } = true;

        public bool Log(Alert alert)
        {
            if (alert is null) return false;
            return Log(alert as Exception);
        }

        private static bool Log(Exception exception)
        {
            Console.WriteLine(exception.ToString());
            return true;
        }

        public bool Log(string message)
        {
            Console.WriteLine(message);
            return true;
        }

        public static Alert SolvingStarted() => new Alert("Solving", AlertLevel.Info, "Solving info", "Solving Started");
        public static Alert SolvingCompleted() => new Alert("Solving", AlertLevel.Info, "Solving info", "Solving Completed");
        public static Alert PromptConfirmationAlert(string topic) => new Alert("", AlertLevel.Info, $"Load {topic} ?", $"Press [y/n]");
    
    }
}
