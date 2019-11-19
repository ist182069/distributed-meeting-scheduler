using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS.Commands
{
    class Client : Command
    {        
        string[] words;

        public Client(ref PCSLibrary pcsLibrary) : base(ref pcsLibrary)
        {
            this.words = base.pcsLibrary.GetWords();
        }
        public override object Execute()
        {
            string client_arguments, client_identifier, client_url, client_path, client_script_path, server_url;

            client_identifier = words[1];
            client_url = words[2];
            server_url = words[3];
            client_script_path = words[4];

            client_path = PCSUtils.AssembleCurrentPath(CLIENT) + "\\" + CLIENT_EXE;

            client_arguments = client_identifier + " " +client_url + " " + server_url + " " + client_script_path;

            Process client_process = new Process();
            client_process.StartInfo.FileName = client_path;
            client_process.StartInfo.Arguments = client_arguments;
            client_process.Start();

            base.pcsLibrary.AddKeyValueToClientDictionary(client_identifier, client_process);

            return null;
        }
    }
}
