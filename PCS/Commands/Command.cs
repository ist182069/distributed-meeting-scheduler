namespace MSDAD.PCS.Commands
{

    abstract class Command
    {
        public const string CLIENT = "Client";
        public const string CLIENT_EXE = "Client.exe";

        public const string SERVER = "Server";
        public const string SERVER_EXE = "Server.exe";

        public const string SERVER_SCRIPTS = "Server_Scripts";

        public PCSLibrary pcsLibrary;
        public Command(ref PCSLibrary pcsLibrary)
        {
            this.pcsLibrary = pcsLibrary;
        }
        public abstract object Execute();
    }
}
