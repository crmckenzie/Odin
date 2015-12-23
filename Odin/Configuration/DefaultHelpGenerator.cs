using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Configuration
{
    public class DefaultHelpGenerator : HelpGenerator
    {
        public override string GenerateHelp(Command command, string actionName = "")
        {
            var builder = new StringBuilder();
            return GenerateHelp(command, builder, actionName);
        }

        private string GenerateHelp(Command command, StringBuilder builder, string actionName = "")
        {
            if (command.Actions.ContainsKey(actionName))
            {
                var action = command.Actions[actionName];
                return Help(action);
            }

            if (command.SubCommands.ContainsKey(actionName))
            {
                var subCommand = command.SubCommands[actionName];
                return GenerateHelp(subCommand, builder);
            }

            builder.AppendLine(command.Description);

            if (command.SubCommands.Any())
                GetSubCommandsHelp(command, builder);

            if (command.Actions.Any())
                GetMethodsHelp(command, builder);

            var result = builder.ToString();

            return result;

        }

        private string Help(MethodInvocation action)
        {
            var builder = new System.Text.StringBuilder();
            var name = action.IsDefault ? $"{action.Name} (default)" : action.Name;

            builder.AppendLine($"{name,-30}{action.GetDescription()}");

            foreach (var parameter in action.ParameterValues)
            {
                var description = parameter.GetDescription();
                builder.Append($"\t{parameter.LongOptionName,-26}");
                if (parameter.HasAliases())
                {
                    var aliases = string.Join(", ", parameter.GetAliases());
                    var text = $"aliases: {aliases}";
                    builder.AppendLine(text)
                        .Append($"\t{new string(' ', 26)}")
                        ;
                }

                builder.AppendLine(description);
            }
            return builder.ToString();
        }

        private void GetMethodsHelp(Command command, StringBuilder builder)
        {
            builder
                .AppendLine()
                .AppendLine()
                .AppendLine("ACTIONS");

            foreach (var method in command.Actions.Values.OrderBy(m => m.Name))
            {
                var methodHelp = Help(method); ;
                builder.AppendLine(methodHelp);
            }

            builder.AppendLine();
            builder.AppendLine("To get help for actions");
            var helpActionName = command.Conventions.GetActionName(command.GetType().GetMethod("Help"));
            builder.AppendFormat("\t{0} {1} <action>", command.Name, helpActionName)
                .AppendLine();
        }

        private void GetSubCommandsHelp(Command command, StringBuilder builder)
        {
            builder
                .AppendLine()
                .AppendLine()
                .AppendLine("SUB COMMANDS");

            foreach (var subCommand in command.SubCommands.Values)
            {
                builder
                    .AppendFormat("{0,-30}", subCommand.Name)
                    .AppendLine(subCommand.Description)
                    ;
            }

            builder.AppendLine();
            builder.AppendLine("To get help for subcommands");
            var helpActionName = command.Conventions.GetActionName(command.GetType().GetMethod("Help"));

            if (command.IsRoot())
            {
                builder.AppendFormat("\t{0} <subcommand>", helpActionName);
            }
            else
            {
                var fullPath = command.GetFullCommandPath().Skip(1); // remove the root command from the path.
                var path = string.Join(" ", fullPath);
                builder.AppendFormat("\t{0} {1} <subcommand>", path, helpActionName);
            }

        }
    }
}
