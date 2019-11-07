using MSDAD.Client.Exceptions;
using MSDAD.Client.Commands.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MSDAD.Client.Commands;
using MSDAD.Client.Commands.Parser;

namespace MSDAD.Client
{
    class ClientParser
    {
        const string PING_COMMAND = "ping";
        const string CREATE = "create";
        const string EXIT = "exit";
        const string LIST = "list";
        const string JOIN = "join";
        const string CLOSE = "close";

        int port_int;
        string client_url, script_name, server_url, user_identifier, ip_string, port_string;
        
        ClientLibrary clientLibrary;
        Command command;

        public ClientParser(string script_name)
        {
            

            this.ip_string = ClientUtils.GetLocalIPAddress();
            Console.Write("Pick a client port: ");

            this.port_string = Console.ReadLine();
            this.port_int = Int32.Parse(port_string);
            // TODO adicionar excepcao aqui

            Console.Write("Pick a user identifier: ");
            this.user_identifier = Console.ReadLine();

            Console.Write("Type the server identifier to whom you want to connect: ");
            this.server_url = Console.ReadLine();

            this.clientLibrary = new ClientLibrary(user_identifier, server_url, ip_string, port_int);
            new Initialize(ref this.clientLibrary);

            this.script_name = script_name;
        }

        public ClientParser(string client_url, string server_url, string script_name)
        {
            int port;
            string client_identifier, port_string;
            string[] split_url, split_ip;

            split_url = client_url.Split('/');
            split_ip = split_url[2].Split(':');
            
            this.ip_string = split_ip[0];            
            port_string = split_ip[1];

            port = Int32.Parse(port_string);

            client_identifier = split_url[3];

            this.client_url = client_url;
            this.server_url = server_url;

            Console.WriteLine(client_identifier);
            Console.WriteLine(this.server_url);
            Console.WriteLine(this.ip_string);
            Console.WriteLine(port);
            Console.WriteLine(script_name);
            this.clientLibrary = new ClientLibrary(client_identifier, this.server_url, this.ip_string, port);

            new Initialize(ref this.clientLibrary);

            this.script_name = script_name;
        }
        public void Parse()
        {            
            string script_path;

            script_path = this.AssembleScript();

            Console.WriteLine(script_path);

            if (this.ScriptExists(script_path))
            {
                int counter = 0;
                string line;

                System.IO.StreamReader file = new System.IO.StreamReader(script_path);
                while ((line = file.ReadLine()) != null)
                {
                    this.ParseLine(line);
                    counter++;
                }
                file.Close();

            }
            else
            {
                throw new ClientLocalException(ErrorCodes.NONEXISTENT_SCRIPT);
            }

            while (true) ;
        }

        private bool ScriptExists(string script_path)
        {
            bool result;

            if (File.Exists(script_path))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        private string AssembleScript()
        {
            string current_path;

            current_path = ClientUtils.AssembleCurrentPath() + "\\" + "Scripts" + "\\" + this.script_name;            

            return current_path;
        }
        private void ParseLine(string text_line)
        {
            string[] words = text_line.Split(' ');

            switch(words[0])
            {
                case CREATE:
                    new Create(ref this.clientLibrary, words).Execute();
                    break;
                case LIST:
                    new List(ref this.clientLibrary).Execute();
                    break;
                case JOIN:
                    new Join(ref this.clientLibrary, words).Execute();
                    break;
                case CLOSE:
                    new Close(ref this.clientLibrary, words).Execute();
                    break;
                default:
                    Console.WriteLine(ErrorCodes.INVALID_COMMAND);
                    break;
            }
        }
  
    }
}
