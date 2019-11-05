using System;
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
            
            Console.WriteLine(result);

            return result;
        }
    }
}
