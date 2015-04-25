using System;
using EasyWIFI.Lib.ICS;

namespace EasyWIFI.Lib.Host
{
    public class SharableConnection
    {
        public SharableConnection() { }

        public SharableConnection(IcsConnection conn)
        {
            this.Name = conn.Name;
            this.DeviceName = conn.DeviceName;
            this.Guid = conn.Guid;
        }

        public string Name { get; set; }
        public string DeviceName { get; set; }
        public Guid Guid { get; set; }
    }
}
