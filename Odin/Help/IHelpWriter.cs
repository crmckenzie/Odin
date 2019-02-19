namespace Odin.Help
{
    /// <summary>
    /// Provides a point of customization for writing out help documentation.
    /// </summary>
    public interface IHelpWriter
    {
        /// <summary>
        /// Return a string containing the help documentation. Help docs should be
        /// scoped to actionName if specified.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        string Write(Command command, string actionName = "");

    }
}