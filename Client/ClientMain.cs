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
                    ClientParser client_parser;
                    client_parser = new ClientParser(null);
                    client_parser.Parse();
                }
                else if(args.Length==1)
                {
                    ClientParser client_parser;
                    client_parser = new ClientParser(args[0]);
                    client_parser.Parse();
                }        
                else if(args.Length==3)
                {
                    ClientParser client_parser;
                    client_parser = new ClientParser(args[0], args[1], args[2]);
                    client_parser.Parse();
                }
                else
                {
                    throw new ClientLocalException("Error! Invalid number of arguments...");
                }
            }
        }
    }
    
}
