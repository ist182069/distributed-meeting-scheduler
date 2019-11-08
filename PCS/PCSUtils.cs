using MSDAD.Library;
using MSDAD.PCS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS
{
    class PCSUtils : CommonUtils
    {
        private const string CLIENT = "Client";
        private const string SERVER = "Server";
        private const string SERVER_SCRIPTS = "Server_Scripts";

        public static string AssembleCurrentPath(string option)
        {
            string path = null;

            switch (option)
            {
                case CLIENT:
                    path = Path(CLIENT);
                    break;
                case SERVER:
                    path = Path(SERVER);
                    break;
                case SERVER_SCRIPTS:
                    path = Path(SERVER_SCRIPTS);
                    break;
            }

            return path;
        }


        private static string Path(string option)
        {
            string server_path;
            string[] current_path;

            if(option.Equals(CLIENT) || option.Equals(SERVER))
            {
                current_path = System.AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\PCS\\bin\\Debug" }, StringSplitOptions.None);
                server_path = current_path[0] + "\\" + option + "\\bin\\Debug";
                return server_path;
            } else if(option.Equals(SERVER_SCRIPTS))
            {                
                current_path = System.AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\PCS\\bin\\Debug" }, StringSplitOptions.None);
                server_path = current_path[0] + "\\" + SERVER + "\\Locations";
                return server_path;
            } else
            {
                throw new PCSException("Error: \"" + option + "\" does not exist...");
            }            
        }
    }
}
