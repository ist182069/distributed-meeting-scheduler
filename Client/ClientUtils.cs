using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.Client
{
    class ClientUtils
    {
        public static string GetLocalIPAddress()
        {
            string client_ip = null;

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    client_ip = ip.ToString();
                }
            }

            return client_ip;
        }

        public static string AssembleClientAddress(string ip, int port)
        {
            string client_address;

            client_address = ip + ":" + port;

            return client_address;
        }

        public static string AssembleCurrentPath()
        {
            string[] current_path;
            current_path = System.AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\Debug" }, StringSplitOptions.None);
            return current_path[0];
        }
    }
}
