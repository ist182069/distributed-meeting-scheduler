using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MSDAD.Library
{
    public class CommonUtils
    {
        public static string GetLocalIPAddress()
        {
            string local_ip = null;

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    local_ip = ip.ToString();
                }
            }

            return local_ip;
        }

        public static string AssembleAddress(string ip, int port)
        {
            string address;

            address = ip + ":" + port;

            return address;
        }

        public static string AssembleCurrentPath()
        {
            string[] current_path;
            current_path = System.AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\Debug" }, StringSplitOptions.None);
            return current_path[0];
        }

        public static bool ValidateAddress(string address)
        {
            bool result;
            Regex pattern;

            pattern = new Regex("^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):[0-9]+$");
            result = pattern.IsMatch(address);

            // If does not work tries for localhost
            if (!result)
            {
                pattern = new Regex("^localhost:[0-9]+$");
                result = pattern.IsMatch(address);
            }

            Console.WriteLine(result);

            return result;
        }

        public static int GetPortFromUrl(string url)
        {
            int port;
            string[] split_url;

            split_url = url.Split(new string[] { "tcp://" }, StringSplitOptions.None);
            split_url = split_url[1].Split('/');
            split_url = split_url[0].Split(':');

            port = Int32.Parse(split_url[1]);

            return port;
        }

        public static string GetIPFromUrl(string url)
        {
            string ip;
            string[] split_url;

            split_url = url.Split(new string[] { "tcp://" }, StringSplitOptions.None);
            split_url = split_url[1].Split('/');
            split_url = split_url[0].Split(':');

            ip = split_url[0];

            return ip;
        }

        public static string GetRemotingIdFromUrl(string url)
        {
            string id;
            string[] split_url;

            split_url = url.Split(new string[] { "tcp://" }, StringSplitOptions.None);
            split_url = split_url[1].Split('/');

            id = split_url[1];

            return id;
        }

        public static string AssembleRemotingURL(string ip, int port, string remoting_identifier)
        {
            string tcp_url;

            tcp_url = "tcp://" + ip + ":" + port + "/" + remoting_identifier;

            return tcp_url;
        }
    }
}


