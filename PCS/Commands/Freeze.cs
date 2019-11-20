using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSDAD.PCS.Commands
{
    class Freeze : Command
    {        
        string[] words;
        public Freeze(ref PCSLibrary pcsLibrary) : base(ref pcsLibrary)
        {
            this.words = base.pcsLibrary.GetWords();
        }
        public override object Execute()
        {
            string server_identifier;
            Process serverProcess;
            Dictionary<string, Tuple<string, Process>> server_dictionary;

            server_identifier = words[1];

            server_dictionary = this.pcsLibrary.GetServerDictionary();

            if (server_dictionary.ContainsKey(server_identifier))
            {
                serverProcess = server_dictionary[server_identifier].Item2;

                foreach (ProcessThread pT in serverProcess.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }

                    SuspendThread(pOpenThread);

                    CloseHandle(pOpenThread);
                }
            }

            return null;
        }
    }
}
