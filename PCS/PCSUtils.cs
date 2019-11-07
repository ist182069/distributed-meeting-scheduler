using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS
{
    class PCSUtils
    {
        private const string CLIENT = "Client";
        private const string SERVER = "Server";

        public static string AssemblePath(string option)
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
            }

            return path;
        }


        private static string Path(string option)
        {
            string server_path;
            string[] current_path;

            if (System.Diagnostics.Debugger.IsAttached)
            {

                current_path = System.AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\PCS\\bin\\Debug" }, StringSplitOptions.None);
                server_path = current_path[0] + "\\" + option + "\\bin\\Debug";
                return server_path;
            }
            else
            {
                current_path = System.AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\PCS" }, StringSplitOptions.None);
                server_path = current_path[0] + "\\" + option;
                return server_path;
            }
        }
    }
}
