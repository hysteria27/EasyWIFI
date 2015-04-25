using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using EasyWIFI.Lib.ICS;
using EasyWIFI.Lib.WLAN;

namespace EasyWIFI.Lib.Host
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class EasyWIFIHost : IEasyWIFIHost
    {
        private WlanManager wlanManager;
        private IcsManager icsManager;

        private SharableConnection currentSharedConnection;

        public EasyWIFIHost()
        {
            this.wlanManager = new WlanManager();
            this.icsManager = new IcsManager();
        }

        #region IEasyWIFIHost Members
        private string _lastErrorMessage;//////////////////////////
        public string GetLastError()
        {
            return this._lastErrorMessage;
        }//////////////////////////////////////////////////////////

        public bool Start(SharableConnection sharedConnection)
        {
            try
            {
                this.Stop();

                if (sharedConnection != null)
                {
                    if (sharedConnection.Guid != Guid.Empty)
                    {
                        if (this.icsManager.SharingInstalled)
                        {
                            var privateConnectionGuid = this.wlanManager.HostedNetworkInterfaceGuid;

                            if (privateConnectionGuid == Guid.Empty)
                            {
                                // If the GUID for the Hosted Network Adapter isn't return properly,
                                // then retrieve it by the DeviceName.
                                try
                                {
                                    privateConnectionGuid = (from c in this.icsManager.Connections
                                                             where c.props.DeviceName.ToLowerInvariant().Contains("microsoft virtual wifi miniport adapter") // Windows 7
                                                                || c.props.DeviceName.ToLowerInvariant().Contains("microsoft hosted network virtual adapter") // Windows 8
                                                             select c.Guid).First();
                                    // Note: This may not work correctly if there are multiple wireless adapters within the computer.
                                }
                                catch
                                {
                                    // Device still not found, so throw exception so the message gets raised up to the client.
                                    throw new Exception("Virtual Wifi device not found!\n\nNeither \"Microsoft Hosted Network Virtual Adapter\" or \"Microsoft Virtual Wifi Miniport Adapter\" were found.");
                                }

                                //privateConnectionGuid = (from c in this.icsManager.Connections
                                //                         where c.props.DeviceName.ToLowerInvariant().Contains("microsoft virtual wifi miniport adapter") // Windows 7
                                //                            || c.props.DeviceName.ToLowerInvariant().Contains("microsoft hosted network virtual adapter") // Windows 8
                                //                         select c.Guid).First();
                                //// Note: This may not work correctly if there are multiple wireless adapters within the computer.

                                //if (privateConnectionGuid == Guid.Empty)//////////////////////////////////////
                                //{
                                //    // Device still not found, so throw exception so the message gets raised up to the client.
                                //    throw new Exception("Virtual Wifi device not found!\n\nNeither \"Microsoft Hosted Network Virtual Adapter\" or \"Microsoft Virtual Wifi Miniport Adapter\" were found.");
                                //}/////////////////////////////////////////////////////////////////////////////
                            }

                            this.icsManager.EnableIcs(sharedConnection.Guid, privateConnectionGuid);

                            this.currentSharedConnection = sharedConnection;
                        }
                    }
                }
                else
                {
                    this.currentSharedConnection = null;
                }

                this.wlanManager.StartHostedNetwork();

                return true;
            }
            catch (Exception ex)///////////////<<<<ini doang
            {
                this._lastErrorMessage = ex.Message;///////////////<<<<ini doang
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (this.icsManager.SharingInstalled)
                {
                    this.icsManager.DisableIcsOnAll();
                }

                this.wlanManager.StopHostedNetwork();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetConnectionSettings(string ssid, int maxNumberOfPeers)
        {
            try
            {
                this.wlanManager.SetConnectionSettings(ssid, maxNumberOfPeers);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ConnectionSettings GetConnectionSettings()
        {
            try
            {
                string ssid;
                int maxNumberOfPeers;

                var r = this.wlanManager.QueryConnectionSettings(out ssid, out maxNumberOfPeers);

                return new ConnectionSettings()
                {
                    SSID = ssid,
                    MaxPeerCount = maxNumberOfPeers
                };
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<SharableConnection> GetSharableConnections()
        {
            List<IcsConnection> connections;
            try
            {
                connections = this.icsManager.Connections;
            }
            catch
            {
                connections = new List<IcsConnection>();
            }

            // Empty item to signify No Connection Sharing
            yield return new SharableConnection() { DeviceName = "No Internet Sharing", Guid = Guid.Empty, Name = "No Internet Sharing" };

            if (connections != null)
            {
                foreach (var conn in connections)
                {
                    if (conn.IsConnected && conn.IsSupported)
                    {
                        yield return new SharableConnection(conn);
                    }
                }
            }
        }

        public bool SetPassword(string password)
        {
            try
            {
                this.wlanManager.SetSecondaryKey(password);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetPassword()
        {
            try
            {
                string passKey = string.Empty;
                bool isPassPhrase;
                bool isPersistent;

                var r = this.wlanManager.QuerySecondaryKey(out passKey, out isPassPhrase, out isPersistent);

                return passKey;
            }
            catch
            {
                return null;
            }
        }

        public bool IsStarted()
        {
            try
            {
                return wlanManager.IsHostedNetworkStarted;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<ConnectedPeer> GetConnectedPeers()
        {
            foreach (var v in wlanManager.Stations)
            {
                yield return new ConnectedPeer(v.Value);
            }
        }

        public SharableConnection GetSharedConnection()
        {
            return this.currentSharedConnection;
        }
        #endregion
    }
}
