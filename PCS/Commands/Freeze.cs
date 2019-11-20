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
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);



        string[] words;
        public Freeze(ref PCSLibrary pcsLibrary) : base(ref pcsLibrary)
        {
            this.words = base.pcsLibrary.GetWords();
        }
        public override object Execute()
        {
            string server_identifier;
            Process serverProcess;
            Dictionary<string, Process> server_dictionary;

            server_identifier = words[1];

            server_dictionary = this.pcsLibrary.GetServerDictionary();

            if (server_dictionary.ContainsKey(server_identifier))
            {
                serverProcess = server_dictionary[server_identifier];

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
