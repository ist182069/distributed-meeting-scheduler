namespace MSDAD.PCS
{
    class PCSMain
    {
        private const int DEFAULT_PORT = 10000;

        static void Main(string[] args)
        {
            PCSParser pcsParser;
            pcsParser = new PCSParser();
            pcsParser.Start(DEFAULT_PORT);
            pcsParser.WaitForCommands();

            while (true) ;
        }       
    }
}
