using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATUPNPLib;
using NETCONLib;
using NetFwTypeLib;
using System.IO;

namespace FingerPrintClockServer
{
    class Program
    {
        private const string CLSID_FIREWALL_MANAGER = "{304CE942-6E39-40D8-943A-B913C40C9CD4}";
        private const string PROGID_OPEN_PORT = "HNetCfg.FWOpenPort";

        //Profile Type
        const int NET_FW_PROFILE2_DOMAIN = 1;
        const int NET_FW_PROFILE2_PRIVATE = 2;
        const int NET_FW_PROFILE2_PUBLIC = 4;
        const int NET_FW_PROFILE2_ALL = 2147483647;

        public static void Main(string[] args)
        {
            try
            {
                bool bnAddPort = true;
                string strPortName = "Validite Web Service Port";
              
                INetFwMgr manager = GetFirewallManager();

                INetFwPolicy2 fwPolicy2;
                Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
                fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);

                foreach (INetFwRule rule in fwPolicy2.Rules)
                {
                    if (rule.Name == strPortName)
                    {
                        bnAddPort = false;
                        break;
                    }
                }

                if (bnAddPort == true)
                {
                    Type type = Type.GetTypeFromProgID(PROGID_OPEN_PORT);
                    INetFwOpenPort port = Activator.CreateInstance(type) as INetFwOpenPort;

                    port.Name = strPortName;
                    port.Port = 8000;
                    port.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                    port.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                    port.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY;
                 
                    manager.LocalPolicy.CurrentProfile.GloballyOpenPorts.Add(port);

                    var Rule = fwPolicy2.Rules.Item(strPortName);

                    //Set Rule to All
                    Rule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
                }
            }
            catch (Exception e)
            {
            }
        }

        private static NetFwTypeLib.INetFwMgr GetFirewallManager()
        {
            Type objectType = Type.GetTypeFromCLSID(new Guid(CLSID_FIREWALL_MANAGER));
            return Activator.CreateInstance(objectType) as NetFwTypeLib.INetFwMgr;
        }
    }
}
