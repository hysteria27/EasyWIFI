using EasyWIFI.Lib.WLAN;

namespace EasyWIFI.Lib.Host
{
    public class ConnectedPeer
    {
        public ConnectedPeer()
        {
        }

        public ConnectedPeer(WlanStation peer)
            : this()
        {
            this.MacAddress = peer.MacAddress;
        }

        public string MacAddress { get; set; }

        public string Type { get; set; }

        public string IPAddress { get; set; }
    }
}
