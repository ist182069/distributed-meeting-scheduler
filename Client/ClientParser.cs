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
using MSDAD.Library;
using MSDAD.Client.Commands.CLI;

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

        int client_port = 0;
        string client_url, script_name, server_url, client_identifier, client_ip, client_port_string;
        
        ClientLibrary client_library;

        public ClientParser(string script_name)
        {
            this.client_ip = ClientUtils.GetLocalIPAddress();
            bool client_port_isnt_taken = false;
            while (!client_port_isnt_taken)
            {
                try
                {
                    bool client_port_is_correct = false;
                    while (!client_port_is_correct)
                    {
                        try
                        {
                            Console.Write("Pick a client port: ");
                            this.client_port_string = Console.ReadLine();
                            this.client_port = Int32.Parse(client_port_string);
                            client_port_is_correct = true;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine(ErrorCodes.INVALID_PORT_FORMAT);
                        }
                    }

                    bool client_identifier_is_correct = false;
                    while (!client_identifier_is_correct)
                    {
                        try
                        {
                            Console.Write("Pick a user identifier: ");
                            this.client_identifier = Console.ReadLine();

                            Console.Write("Type the server identifier to whom you want to connect: ");
                            this.server_url = Console.ReadLine();

                            this.client_library = new ClientLibrary(client_identifier, server_url, client_ip, client_port);
                            new Initialize(ref this.client_library).Execute();

                            Console.WriteLine("Success!");

                            client_identifier_is_correct = true;                            
                        }
                        catch (ServerCoreException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    client_port_isnt_taken = true;
                }
                catch (ClientLocalException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            this.script_name = script_name;
        }

        public ClientParser(string client_url, string server_url, string script_name)
        {
            int port;
            string client_identifier, port_string;
            string[] split_url, split_ip;

            split_url = client_url.Split('/');
            split_ip = split_url[2].Split(':');
            
            this.client_ip = split_ip[0];            
            port_string = split_ip[1];

            port = Int32.Parse(port_string);

            client_identifier = split_url[3];

            this.client_url = client_url;
            this.server_url = server_url;

            Console.WriteLine(client_identifier);
            Console.WriteLine(this.server_url);
            Console.WriteLine(this.client_ip);
            Console.WriteLine(port);
            Console.WriteLine(script_name);
            this.client_library = new ClientLibrary(client_identifier, this.server_url, this.client_ip, port);

            new Initialize(ref this.client_library).Execute();

            this.script_name = script_name;
        }
        public void Parse()
        {            
            string script_path;

            script_path = this.AssembleScript();

            Console.WriteLine(script_path);

            if (script_path != null && this.ScriptExists(script_path))
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
            else if(script_path != null)
            {
                throw new ClientLocalException(ErrorCodes.NONEXISTENT_SCRIPT);
            }

            string input;

            while (true)
            {
                Console.Write("Insert the command you want to run on the Meeting Scheduler: ");
                input = Console.ReadLine();

                try
                {
                    switch (input)
                    {
                        case PING_COMMAND:
                            new Ping(ref client_library).Execute();
                            break;
                        case CREATE:
                            new Commands.CLI.Create(ref client_library).Execute();
                            break;
                        case LIST:
                            new List(ref client_library).Execute();
                            break;
                        case JOIN:
                            new Commands.CLI.Join(ref client_library).Execute();
                            break;
                        case CLOSE:
                            new Commands.CLI.Close(ref client_library).Execute();
                            break;
                        case EXIT:
                            Console.WriteLine("Bye!");
                            return;
                        default:
                            Console.WriteLine(ErrorCodes.INVALID_COMMAND);
                            break;
                    }
                }
                catch (Exception exception) when (exception is ClientLocalException || exception is ServerCoreException)
                {
                    Console.WriteLine(exception.Message);
                }

            }
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
            
            if(this.script_name == null)
            {
                return null;
            }
            else
            {
                current_path = ClientUtils.AssembleCurrentPath() + "\\" + "Scripts" + "\\" + this.script_name;

                return current_path;
            }
        }
        private void ParseLine(string text_line)
        {
            string[] words = text_line.Split(' ');

            switch(words[0])
            {
                case CREATE:
                    new Commands.Parser.Create(ref this.client_library, words).Execute();
                    break;
                case LIST:
                    new List(ref this.client_library).Execute();
                    break;
                case JOIN:
                    new Commands.Parser.Join(ref this.client_library, words).Execute();
                    break;
                case CLOSE:
                    new Commands.Parser.Close(ref this.client_library, words).Execute();
                    break;
                default:
                    Console.WriteLine(ErrorCodes.INVALID_COMMAND);
                    break;
            }
        }
  
    }
}
