using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS.Commands
{
    class Crash : Command
    {
        string[] words;

        public Crash(ref PCSLibrary pcsLibrary) : base(ref pcsLibrary)
        {
            words = pcsLibrary.GetWords();
        }
        public override object Execute()
        {
            string server_identifier;
            Process serverProcess;
            Dictionary<string, Tuple<string, Process>> server_dictionary;

            server_identifier = words[1];

            server_dictionary = this.pcsLibrary.GetServerDictionary();

            if(server_dictionary.ContainsKey(server_identifier))
            {
                serverProcess = server_dictionary[server_identifier].Item2;
                serverProcess.Kill();
                server_dictionary.Remove(server_identifier);
            }            

            return null;
        }
    }
}
