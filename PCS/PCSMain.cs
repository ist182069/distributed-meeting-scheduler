namespace MSDAD.PCS
{
    class PCSMain
    {
        private const int DEFAULT_PORT = 1001;

        static void Main(string[] args)
        {
            PCSParser pcsParser;
            pcsParser = new PCSParser();
            pcsParser.Start(DEFAULT_PORT);
            //pcsParser.WaitForCommands();

            while (true) ;
        }       
    }
}
