namespace MSDAD
{
    namespace Client
    {
        class ClientMain
        {            
            static void Main(string[] args)
            {
                ClientCLI clientUI;

                clientUI = new ClientCLI();
                clientUI.Display();
            }
        }
    }
    
}
