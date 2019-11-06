namespace MSDAD.PCS.Commands
{

    abstract class Command
    {
        public PCSLibrary pcsLibrary;
        public Command(ref PCSLibrary pcsLibrary)
        {
            this.pcsLibrary = pcsLibrary;
        }
        public abstract object Execute();
    }
}
