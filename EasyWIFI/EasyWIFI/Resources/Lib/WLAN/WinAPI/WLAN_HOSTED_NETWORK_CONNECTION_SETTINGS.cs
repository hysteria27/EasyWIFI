﻿using System;
using System.Runtime.InteropServices;

namespace EasyWIFI.Lib.WLAN.WinAPI
{
    [StructLayout(LayoutKind.Sequential)] //, CharSet =  CharSet.Unicode)]
    public struct WLAN_HOSTED_NETWORK_CONNECTION_SETTINGS
    {
        public DOT11_SSID hostedNetworkSSID;
        public UInt32 dwMaxNumberOfPeers; // DWORD
    }
}
