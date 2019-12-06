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

            try
            {
                new Initialize(ref this.client_library).Execute();
            }
            catch (ServerCoreException sce)
            {
                Console.WriteLine("\r\nCould not properly initialize the client! Aborting...");
                Console.WriteLine(sce.Message);
            }
            this.script_name = client_arguments_split[3];
        }

        public ClientParser(string client_identifier, string client_url, string server_url, string script_name)
        {
            this.server_url = server_url;
            this.client_ip = ClientUtils.GetIPFromUrl(client_url);
            this.client_remoting = ClientUtils.GetRemotingIdFromUrl(client_url);
            this.client_port = ClientUtils.GetPortFromUrl(client_url);            

            this.client_library = new ClientLibrary(client_identifier, this.client_remoting, server_url, this.client_ip, this.client_port);
            try
            {
                new Initialize(ref this.client_library).Execute();
            }
            catch (ServerCoreException sce)
            {
                Console.WriteLine("Could not properly initialize the client! Aborting...");
                Console.WriteLine(sce.StackTrace);
            }
                
            this.script_name = script_name;
        }
        public void Parse()
        {            
            string script_path;

            script_path = this.AssembleScript();

            Console.WriteLine("The script was found on path: \"" + script_path + "\".");

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
            Console.WriteLine("The parse command was: " + text_line);
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
