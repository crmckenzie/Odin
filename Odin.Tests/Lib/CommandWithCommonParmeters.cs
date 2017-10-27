namespace Odin.Tests.Lib
{
    using Odin.Attributes;

    public class CommandWithCommonParmeters : Command
    {   
        [Parameter]
        [Alias("t")]
        public string Text { get; set; }

        [Action]
        public void Display(string subject = null)
        {
            var text = subject ?? this.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                this.Logger.Info("none");
            }
            else
            {
                this.Logger.Info(text);
            }
        }
    }
}