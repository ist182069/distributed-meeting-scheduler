using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace MSDAD.Server
{
    class ServerUtils
    {
        public static string AssembleClientAddress(string ip, int port)
        {
            string client_address;

            client_address = ip + ":" + port;

            return client_address;
        }

        public static bool ValidateAddress(string client_address)
        {
            bool result;            
            Regex pattern;

            pattern = new Regex("^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?):[0-9]+$");
            result = pattern.IsMatch(client_address);
            
            // If does not work tries for localhost
            if(!result)
            {
                pattern = new Regex("^localhost:[0-9]+$");
                result = pattern.IsMatch(client_address);
            }

            Console.WriteLine(result);

            return result;
        }

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

        public static string GetServerIdFromUrl(string url)
        {
            string id;
            string[] split_url;

            split_url = url.Split(new string[] { "tcp://" }, StringSplitOptions.None);
            split_url = split_url[1].Split('/');

            id = split_url[1];
            
            return id;
        }
    }
}
