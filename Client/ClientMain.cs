using System;
using MSDAD.Client.Exceptions;

namespace MSDAD
{
    namespace Client
    {
        class ClientMain
        {            
            static void Main(string[] args)
            {            
                if(args.Length==0)
                {
                    ClientCLI clientUI;
                    clientUI = new ClientCLI();
                    clientUI.Display();
                }
                else if(args.Length==1)
                {
                    ClientParser clientParser;
                    clientParser = new ClientParser(args[0]);
                    clientParser.Parse();
                }        
                else if(args.Length==3)
                {
                    ClientParser clientParser;
                    clientParser = new ClientParser(args[0], args[1], args[2]);
                    clientParser.Parse();
                }
                else
                {
                    throw new ClientLocalException("Error! Invalid number of arguments...");
                }
            }
        }
    }
    
}
