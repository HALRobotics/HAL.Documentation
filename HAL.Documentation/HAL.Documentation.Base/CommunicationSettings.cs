using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.Base
{
   public class CommunicationSettings
    {
       public CommunicationSettings()
        {
            IpSettings = new List<IpSettings>();
        }

        public List<IpSettings> IpSettings { get; set; }
    }
}
