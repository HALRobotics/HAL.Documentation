using HAL.Communications;
using HAL.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.KaplaPlusCamera
{
   public class CameraController : Controller
    {
        public CameraController(Identifier identity = null, List<Protocol> protocols = null)
           : base(identity ?? new Identifier("Optitrack"), protocols ?? new List<Protocol> { }) { }


        protected CameraController(CameraController clonee) : base(clonee) { }
       

        /// <inheritdoc />
        public override Controller Clone() => new CameraController(this);

    }

    
}
