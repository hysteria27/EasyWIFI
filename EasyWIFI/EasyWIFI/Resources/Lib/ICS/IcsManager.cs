﻿using System;
using System.Collections.Generic;
using System.Linq;
using NETCONLib;

namespace EasyWIFI.Lib.ICS
{
    public class IcsManager
    {
        protected INetSharingManager _NSManager;

        public IcsManager()
        {
            this.Init();
        }

        public void Init()
        {
            this._NSManager = new NetSharingManagerClass();
        }

        public void EnableIcs(Guid publicGuid, Guid privateGuid)
        {
            if (!this.SharingInstalled)
            {
                throw new Exception("Internet Connection Sharing NOT Installed");
            }

            var connections = this.Connections;

            IcsConnection publicConn = (from c in connections
                                        where c.IsMatch(publicGuid)
                                        select c).First();

            IcsConnection privateConn = (from c in connections
                                         where c.IsMatch(privateGuid)
                                         select c).First();

            this.DisableIcsOnAll();

            publicConn.EnableAsPublic();
            privateConn.EnableAsPrivate();
        }

        public void DisableIcsOnAll()
        {
            foreach (var conn in this.Connections)
            {
                if (conn.IsSupported)
                {
                    conn.DisableSharing();
                }
            }
        }

        private List<IcsConnection> _Connections = null;
        public List<IcsConnection> Connections
        {
            get
            {
                if (this._Connections == null)
                {
                    this._Connections = new List<IcsConnection>();

                    foreach (INetConnection conn in this._NSManager.EnumEveryConnection)
                    {
                        this._Connections.Add(new IcsConnection(this._NSManager, conn));
                    }
                }
                return this._Connections;
            }
        }

        public bool SharingInstalled
        {
            get
            {
                return this._NSManager.SharingInstalled;
            }
        }
    }
}
