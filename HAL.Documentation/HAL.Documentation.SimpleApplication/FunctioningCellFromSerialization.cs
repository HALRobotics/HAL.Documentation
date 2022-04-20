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

namespace HAL.Documentation.SimpleApplication
{
    public class FunctioningCellFromSerialization
    {

        /// Deserializes a cell, 
        /// Creates and assigns a procedure,
        /// Exports the procedure to a local folder or upload it to a physical controller.
        private static async Task Main(string[] args)
        {
            await Run("","",new IpSettings("ControllerAddress","192.168.1.202"));
        }
        public static async Task Run(string sessionPath, string exportPath, IpSettings controllerIp)
        {
            ///Creates a new client and sets the required assemblies. Mandatory step. 
            await new Client(ClientBootSettings.Minimal, Assembly.GetAssembly(typeof(ABBController))).StartAsync();

            var virtualCell = new VirtualCell(null, sessionPath);

            #region Procedure

            ///Settings 

            /// Gets default values for <see cref="MotionSettings"/>.
            var defaultMotionSettings = DefaultSettings.Get<MotionSettings>();

            /// Generates custom settings.
            /// From existing, by cloning, to remove any reference to object.
            var customMotionSettings = defaultMotionSettings.Clone() as MotionSettings;
            /// Motions can be interpreted in the Cartesian space or joint space. It is specified by the Space property <see cref="MotionSpace"/>.
            var customCartesianSettings = defaultMotionSettings.Clone() as MotionSettings;
            customCartesianSettings.Space = MotionSpace.Cartesian;
            /// In Cartesian space, the motions are performed by applying settings relative to the TCP (e.g. linear speed and angular speed of the TCP).
            /// Here are some example on how to set or perform operations on the linear speed.
            customMotionSettings.SpeedSettings.PositionSpeed = (m_s)10;
            customMotionSettings.SpeedSettings.PositionSpeed += (m_s)1;
            customMotionSettings.SpeedSettings.PositionSpeed /= 2;

            ///In joint space, the settings of each joint must be specified. It can be simpler to create settings from scratch.
            ///To generate a specific setting for each joint. Example with joint speeds.
            var individualJointSpeeds = (new double[] { 10, 20, 10, 20, 10, 20 });
            ///To generate all the value at once. Example with joint accelerations. 
            var generalJointAccelerations = Enumerable.Repeat(new Angle((deg)20).ToDouble(), virtualCell.Mechanism.ActiveJoints.Count).ToArray();
            /// Joint settings using the two variables specified before.
            var customJointSettings = new MotionSettings(
                        MotionSpace.Joint, new SpeedSettings(new JointSpeeds(virtualCell.Mechanism.ActiveJoints, individualJointSpeeds)),
                        new AccelerationSettings(new JointAccelerations(virtualCell.Mechanism.ActiveJoints, generalJointAccelerations)),
                        new BlendSettings(new Length((mm)1.0), new Angle((deg)1.0), new Length((mm)1)));



            ///Targets to reach.
            var basicTarget = new Target(new MatrixFrame(new Vector3D(100, 100, 100), RotationMatrix.Identity.RotateAroundX(Math.PI))); ///target from a frame.
            Target.Transform(basicTarget, "", (mm)50, (mm)0, (mm)0, (deg)0, (deg)0, (deg)45, null, out var transformedTarget); /// target from a transformation of a frame.
            var jointPositions = new Target(new JointPositions(new Angle[] { (deg)0, (deg)0, (deg)0, (deg)0, (deg)0, (deg)0 }));/// target from joint position.



            /// Actions to be performed.
            var actions = new List<Action>()
            {
                ///Motions from previously  and default settings
                new MotionAction(virtualCell.Mechanism, basicTarget, defaultMotionSettings,"MinimalMotion"),
                /// Motion from a transformed target and previously created Cartesian motion settings. 
                new MotionAction (virtualCell.Mechanism, transformedTarget , customCartesianSettings, "cartesianMotion"),
                /// Motion from a new joint position and the previously created joint motion settings.
                new MotionAction (virtualCell.Mechanism,jointPositions, customJointSettings," approachJoint"),
                /// Delay the execution
                new WaitTimeAction((s)3, "Pause"),
                /// Custom action :  write the identifier alias as a new line. Can be used to call a procedure predefined in the physical controller.
                new ActionSet(new Identifier("DO_01"))
            };

            ///procedure containing the different actions.
            var procedure = new Procedure("BasicActions", actions);

            /// Assign the new procedure to the mechanism.
            virtualCell.Controller.Assign(virtualCell.Mechanism, procedure);

            #endregion
            virtualCell.CommunicationSettings.IpSettings.Add(controllerIp);
            await virtualCell.Export(controllerIp.IpAddress.ToString());

        }




    }
}
