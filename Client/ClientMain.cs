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
                else
                {
                    throw new ClientLocalException("Error! You cannot insert more than one argument in this program.");
                }
            }
        }
    }
    
}
