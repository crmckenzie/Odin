namespace Odin.Configuration
{
    public interface IHelpWriter
    {
        string Write(Command command, string actionName = "");

    }
}