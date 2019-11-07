namespace MSDAD.PCS
{
    class PCSMain
    {
        private const int DEFAULT_PORT = 10000;

        static void Main(string[] args)
        {
            PCSParser pcs_parser;
            pcs_parser = new PCSParser();
            pcs_parser.Start(DEFAULT_PORT);
            pcs_parser.WaitForCommands();

            while (true) ;
        }       
    }
}
