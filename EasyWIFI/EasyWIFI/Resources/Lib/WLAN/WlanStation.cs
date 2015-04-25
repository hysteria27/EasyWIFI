﻿using EasyWIFI.Lib.WLAN.WinAPI;

namespace EasyWIFI.Lib.WLAN
{
    public class WlanStation
    {
        public WlanStation(WLAN_HOSTED_NETWORK_PEER_STATE state)
        {
            this.State = state;
        }

        public WLAN_HOSTED_NETWORK_PEER_STATE State { get; set; }

        public string MacAddress
        {
            get
            {
                return this.State.PeerMacAddress.ConvertToString();
            }
        }
    }
}
