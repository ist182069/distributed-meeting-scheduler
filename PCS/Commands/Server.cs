using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS.Commands
{
    class Server : Command
    {
        private const string SERVER = "Server";
        private const string SERVER_EXE = "Server.exe";

        string[] words; 

        public Server(ref PCSLibrary pcsLibrary) : base(ref pcsLibrary)
        {
            this.words = base.pcsLibrary.GetWords();
        }
        public override object Execute()
        {
            string server_identifier, server_url, server_path;
            //nao percebo para que servem estes
            int par_1, par_2, par_3;

            server_identifier = words[1];
            server_url = words[2];
            // usar os restantes campos que nao percebo para que sao words[3], words[4], words[5]

            server_path = PCSUtils.AssemblePath(SERVER) + "\\" + SERVER_EXE;

            Process server_process = new Process();
            server_process.StartInfo.FileName = server_path;
            // TODO passar um XML ou whatever para criar as localizacoes
            server_process.StartInfo.Arguments = server_url;
            server_process.Start();            

            base.pcsLibrary.AddKeyValueToServerDictionary(server_identifier, server_process);
            
            return null;
        }
    }
}
