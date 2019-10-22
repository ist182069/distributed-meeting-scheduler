namespace MSDAD
{
    namespace Server
    {
        class ServerMain
        {
            static void Main(string[] args)
            {
                ServerCLI serverCLI;

                serverCLI = new ServerCLI();
                serverCLI.Display();

                System.Console.ReadLine();
            }
        }
    }
    
}
