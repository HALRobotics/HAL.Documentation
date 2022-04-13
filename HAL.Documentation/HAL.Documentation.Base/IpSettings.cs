using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HAL.Documentation.Base
{
    /// <summary>
    /// Class containing the IP address and port of an entity.
    /// </summary>
    public class IpSettings
    {
        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public IpSettings(string alias, IPAddress ipAddress, int? port = null)
        {
            Alias = alias;
            IpAddress = ipAddress;
            Port = port;
        }

        /// <summary>
        /// Constructor with auto parse of the IpAddress.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public IpSettings(string alias, string ipAddress, int? port = null) : this(alias, IPAddress.Parse(ipAddress), port) { }

        #endregion

        #region Properties

        public string Alias { get; set; }
        public IPAddress IpAddress { get; set; }
        public int? Port { get; set; } 
        public string CompleteIpAddress { get => $"{IpAddress}:{ Port}"; }

        #endregion
    }
}
