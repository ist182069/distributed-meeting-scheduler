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
                    client_parser = new ClientParser();
                    client_parser.Parse();
                }     
                else if(args.Length==4)
                {
                    string client_identifier, client_url, client_script, server_url;

                    client_identifier = args[0];
                    client_url = args[1];
                    server_url = args[2];
                    client_script = args[3];

                    ClientParser client_parser;
                    client_parser = new ClientParser(client_identifier, client_url, server_url, client_script);
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
