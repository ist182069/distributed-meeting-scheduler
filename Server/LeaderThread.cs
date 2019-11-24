using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSDAD.Server
{
    class LeaderThread
    {
        public void Run()
        {
            while(true)
            {
                Thread.Sleep(1000*10);
                Console.WriteLine("Sou o lider!");
            }
        }
    }
}
