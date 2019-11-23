using MSDAD.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MSDAD.Client.Commands;
using MSDAD.Library;

namespace MSDAD.Client
{
    class ClientParser
    {
        const string WAIT = "wait";
        const string PING_COMMAND = "ping";
        const string CREATE = "create";
        const string EXIT = "exit";
        const string LIST = "list";
        const string JOIN = "join";        
        const string CLOSE = "close";
        const string STATUS = "Status";

        int client_port = 0;
        string client_arguments, client_url, script_name, server_url, client_identifier, client_ip, client_remoting;
        string[] client_arguments_split, words;
        
        ClientLibrary client_library;

        public ClientParser()
        {
            this.client_ip = ClientUtils.GetLocalIPAddress();
            //bool client_port_isnt_taken = false;

            Console.Write("Type the client parameters within the following format \"c1 tcp://localhost:4001/client1 tcp://localhost:3001/server1 cs1\": ");

            client_arguments = Console.ReadLine();
            client_arguments_split = client_arguments.Split(' ');
            
            this.client_identifier = client_arguments_split[0];
            this.client_url = client_arguments_split[1];
            this.server_url = client_arguments_split[2];

            this.client_ip = ClientUtils.GetIPFromUrl(this.client_url);
            this.client_port = ClientUtils.GetPortFromUrl(this.client_url);
            this.client_remoting = ClientUtils.GetRemotingIdFromUrl(this.client_url);

            this.client_library = new ClientLibrary(this.client_identifier, this.client_remoting, this.server_url, this.client_ip, this.client_port);
            new Initialize(ref this.client_library).Execute();
            this.script_name = client_arguments_split[3];

            /*
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
            */
        }

        public ClientParser(string client_identifier, string client_url, string server_url, string script_name)
        {
            this.server_url = server_url;
            this.client_ip = ClientUtils.GetIPFromUrl(client_url);
            this.client_remoting = ClientUtils.GetRemotingIdFromUrl(client_url);
            this.client_port = ClientUtils.GetPortFromUrl(client_url);            

            this.client_library = new ClientLibrary(client_identifier, this.client_remoting, server_url, this.client_ip, this.client_port);
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

                words = input.Split(' ');

                try
                {
                    switch (words[0])
                    {
                        case PING_COMMAND:
                            new Ping(ref client_library).Execute();
                            break;
                        case CREATE:
                            new Create(ref client_library, words).Execute();
                            break;
                        case LIST:
                            new List(ref client_library).Execute();
                            break;
                        case JOIN:
                            new Join(ref client_library, words).Execute();
                            break;
                        case CLOSE:
                            new Close(ref client_library, words).Execute();
                            break;
                        case STATUS:
                            new Status(ref this.client_library).Execute();
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
            
            if(this.script_name.Equals("0"))
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
            Console.WriteLine(text_line);
            string[] words = text_line.Split(' ');

            switch(words[0])
            {
                case CREATE:
                    new Create(ref this.client_library, words).Execute();
                    break;
                case LIST:
                    new List(ref this.client_library).Execute();
                    break;
                case JOIN:
                    new Join(ref this.client_library, words).Execute();
                    break;
                case CLOSE:
                    new Close(ref this.client_library, words).Execute();
                    break;
                case WAIT:
                    System.Threading.Thread.Sleep(Int32.Parse(words[1]));
                    break;
                case STATUS:
                    new Status(ref this.client_library).Execute();
                    break;
                default:
                    Console.WriteLine(ErrorCodes.INVALID_COMMAND);
                    break;
            }
        }
  
    }
}
