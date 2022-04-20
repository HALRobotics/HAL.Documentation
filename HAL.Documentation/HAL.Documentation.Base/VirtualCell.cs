using HAL.Control;
using HAL.Control.Subsystems.Procedures;
using HAL.Documentation.Base.Monitoring;
using HAL.Objects.Mechanisms;
using HAL.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HAL.Documentation.Base
{
    public class VirtualCell
    {
        #region Constuctors

        /// <summary>
        /// Deserialize a cell from a path and select the first controller and mechanism.
        /// </summary>
        /// <param name="serializedSessionPath">Path from the serialized cell.</param>
        public VirtualCell(Client client, string serializedSessionPath, string exportPath = null) //Todo add: session serilaizer API for public.
        {
            Client = client;
            Path = serializedSessionPath;
            Session = Serialization.Helpers.DeserializeSession(Path, true);
            Client.Sessions.TryAdd(Session.Identity.GUID, Session);
            Controller = Session.ControlGroup.Controllers.OfType<RobotController>().FirstOrDefault();
            Mechanism = Controller.Controlled.OfType<Mechanism>().FirstOrDefault();
            if (Controller != null && Mechanism != null) Controller.AddControlledObject(Mechanism);
            CommunicationSettings = new CommunicationSettings();
            InfoLogger = new Logger();
        }

        #endregion

        #region Properties

        Client Client { get; set; }
        /// Logger to display informations.
        Logger InfoLogger { get; }
        //Serialized session's path.
        string Path { get; }
        public string ExportPath { get; set; }
        public RobotController Controller { get; }
        public Mechanism Mechanism { get; }
        public Runtime.Session Session { get; }
        public CommunicationSettings CommunicationSettings { get; set; }

        #endregion
        
        #region Methods

        public async Task Export(string controllerIp)
        {
           await Solve();
            /// Select a subsystem able to export and upload code.
            var canUpload = Controller.SubsystemManager.TryGetSubsystem<ILoadingCapableSubsystem>(out var uploader);
            if (canUpload)
            {
                uploader.TrySetNetworkIdentity(controllerIp);
                await Controller.TryExportAndUploadAsync(Linguistics.Export.DeclarationMode.Inline, false, cancel: CancellationToken.None);
            }
        } 

        public async Task Solve()
        {
            ///Subscribe to solving events. The logger will display the appropriate alerts when the events are triggered. 
            Session.ControlGroup.Solver.SolvingCompleted += OnSolvingCompleted;
            Session.ControlGroup.Solver.SolvingStarted += OnSolvingStarted;
           
            ///Start the solving. An unsolved procedure cannot be exported nor uploaded.
            Session.ControlGroup.Solver.StartSolution();
            await Controller.TryExportAsync(ExportPath, Linguistics.Export.DeclarationMode.Inline, cancel: CancellationToken.None); 

        }

        #region Events  

        /// Actions to performs when the events are triggered 

        private void OnSolvingStarted(object sender, EventArgs e) //Todo is it ok that this events are not static
        {
            var logged = InfoLogger.Log(Logger.SolvingStarted());
        }

        private void OnSolvingCompleted(object sender, EventArgs e)
        {
            var logged = InfoLogger.Log(Logger.SolvingCompleted());
        }

        #endregion

        #endregion



    }
}
