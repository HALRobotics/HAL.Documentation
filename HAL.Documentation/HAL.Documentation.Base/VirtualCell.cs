using HAL.Control;
using HAL.Documentation.Base.Monitoring;
using HAL.Objects.Mechanisms;
using HAL.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.Base
{
    public class VirtualCell
    {
        /// <summary>
        /// Deserialize a cell from a path and select the first controller and mechanism.
        /// </summary>
        /// <param name="serializedSessionPath">Path from the serialized cell.</param>
        public VirtualCell(Client client, string serializedSessionPath ) //Todo add: session serilaizer API for public.
        {
            Client = client;
            Path = serializedSessionPath;
            Session = Serialization.Helpers.DeserializeSession(Path, true);
            Client.Sessions.TryAdd(Session.Identity.GUID, Session);
            Controller = Session.ControlGroup.Controllers.OfType<RobotController>().FirstOrDefault();
            Mechanism = Controller.Controlled.OfType<Mechanism>().FirstOrDefault();
            if(Controller !=null && Mechanism !=null) Controller.AddControlledObject(Mechanism);
            CommunicationSettings = new CommunicationSettings(); 
        }

        Client Client { get; set; }
        //Serialized session's path.
        string Path { get; }
        public RobotController Controller { get; }
        public Mechanism Mechanism { get; }
        public Runtime.Session Session { get; }
        public CommunicationSettings CommunicationSettings { get; set; }

    }
}
