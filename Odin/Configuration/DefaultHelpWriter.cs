using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Odin.Configuration
{
    /// <summary>
    /// Displays help for a command or action.
    /// </summary>
    public class DefaultHelpWriter : IHelpWriter
    {
        public DefaultHelpWriter()
        {
            this.IdentifierWidth = 20;
            this.SpacerWidth = 4;
            this.IndentWidth = 4;
            this.DescriptionWidth = 52;
        }

        public int PageWidth => this.IdentifierWidth + this.SpacerWidth + this.DescriptionWidth;
        public int IdentifierWidth { get; set; }
        public int SpacerWidth { get; set; }
        public int IndentWidth { get; set; }
        public int DescriptionWidth { get; set; }

        public string Write(Command command, string actionName = "")
        {
            var builder = new StringBuilder();
            WriteCommandHelp(command, builder, actionName);
            return builder.ToString();
        }

        #region commands
        private string[] GetFullCommandPath(Command command)
        {
            var stack = new Stack<string>();
            stack.Push(command.Name);

            var parent = command.Parent;
            while (parent != null)
            {
                stack.Push(parent.Name);
                parent = parent.Parent;
            }

            return stack.ToArray();
        }

        private void WriteCommandHelp(Command command, StringBuilder builder, string actionName = "")
        {
            if (command.Actions.ContainsKey(actionName))
            {
                var action = command.Actions[actionName];
                WriteActionHelp(action, builder);
                return;
            }

            if (command.SubCommands.ContainsKey(actionName))
            {
                var subCommand = command.SubCommands[actionName];
                WriteCommandHelp(subCommand, builder);
                return;
            }

            WriteCommandDescription(command, builder);
            if (command.Actions.Any())
            {
                WriteActionsHelp(command, builder);
            }

            if (command.SubCommands.Any())
            {
                WriteSubCommandsHelp(command, builder);
                WriteSubCommandsHelpFooter(command, builder);
            }
            builder.AppendLine();

        }

        private void WriteCommandDescription(Command command, StringBuilder builder)
        {
            var lines = GetDescription(command)
                .Paginate(this.PageWidth)
                .ToList()
                ;
            lines.ForEach(line => builder.AppendLine(line));

            var defaultAction = command.GetDefaultAction();
            if (defaultAction == null) return;

            builder.AppendLine("");
            builder.Append("default action: ");
            builder.AppendLine(defaultAction.Name);
        }

        private string GetDescription(Command command)
        {
            var builder =new System.Text.StringBuilder();

            var defaultDescription = command.IsRoot() ? "" : command.Name;
            var attribute = command.GetType().GetCustomAttribute<DescriptionAttribute>(inherit: true);

            if (attribute == null)
            {
                builder.AppendLine(defaultDescription);
            }
            else
            {
                builder.AppendLine(attribute.Description);
            }

            return builder.ToString();
        }

        #endregion

        #region actions

        private void WriteActionsHelp(Command command, StringBuilder builder)
        {
            builder
                .AppendLine()
                .AppendLine("ACTIONS");

            foreach (var method in command.Actions.Values.OrderBy(m => m.Name))
            {
                WriteActionHelp(method, builder); ;
            }

            WriteActionFooter(command, builder);
        }

        private void WriteActionHelp(MethodInvocation action, StringBuilder builder)
        {
            WriteActionHeader(action, builder);
            WriteParameters(action, builder);
        }

        private void WriteActionHeader(MethodInvocation action, StringBuilder builder)
        {
            var name = (action.IsDefault ? $"{action.Name}*" : action.Name)
                .PadRight(this.IdentifierWidth, ' ');
            var spacer = new string(' ', this.SpacerWidth);

            var descriptions = GetDescription(action)
                .Paginate(this.DescriptionWidth)
                .ToList()
                ;
            var first = descriptions.FirstOrDefault() ?? "";
            builder.AppendLine($"{name}{spacer}{first}");

            var theRest = descriptions.Skip(1);
            foreach (var descriptionLine in theRest)
            {
                spacer = new string(' ', this.IdentifierWidth + this.SpacerWidth);
                builder.AppendLine($"{spacer}{descriptionLine}");
            }
            builder.AppendLine("");
        }

        private void WriteParameters(MethodInvocation action, StringBuilder builder)
        {
            foreach (var parameter in action.ParameterValues)
            {
                WriteParameter(builder, parameter);
            }
        }

        private void WriteParameter(StringBuilder builder, ParameterValue parameter)
        {
            var indent = new string(' ', this.IndentWidth);
            var spacer = new string(' ', this.SpacerWidth);
            var name = parameter.LongOptionName
                .PadRight(this.IdentifierWidth - this.IndentWidth);

            var descriptions = GetDescription(parameter)
                .Paginate(this.DescriptionWidth)
                .ToList()
                ;

            var first = descriptions.FirstOrDefault() ?? "";
            var line = $"{indent}{name}{spacer}{first}";
            builder.AppendLine(line);

            var theRest = descriptions.Skip(1);
            foreach (var descriptionLine in theRest)
            {
                spacer = new string(' ', this.IdentifierWidth + this.SpacerWidth);
                builder.AppendLine($"{spacer}{descriptionLine}");
            }
            builder.AppendLine("");
        }

        private string GetDescription(MethodInvocation method)
        {
            var builder = new System.Text.StringBuilder();
            if (method.Aliases.Any())
            {
                builder.Append("aliases: ");
                builder.AppendLine(method.Aliases.Join(", "));
            }
            var descriptionAttr = method.MethodInfo.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttr != null)
            {
                builder.AppendLine(descriptionAttr.Description);
            }

            return builder.ToString();
        }

        private string GetDescription(ParameterValue parameter)
        {
            var builder = new System.Text.StringBuilder();

            if (parameter.IsBoolean())
            {
                var line = $"{parameter.Conventions.GetNegatedLongOptionName(parameter.Name)} to negate";
                builder.AppendLine(line);
            }

            if (parameter.ParameterType.IsEnum)
            {
                var values = System.Enum.GetNames(parameter.ParameterType);
                var line = $"valid values: {string.Join(", ", values)}";
                builder.AppendLine(line);
            }

            if (parameter.ParameterInfo.HasDefaultValue)
            {
                builder.Append("default value: ");
                builder.AppendLine(parameter.ParameterInfo.DefaultValue.ToString());
            }

            if (parameter.Aliases.Any())
            {
                builder.Append("aliases: ");
                builder.AppendLine(parameter.Aliases.Join(", "));
            }
            var descriptionAttr = parameter.ParameterInfo.GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttr != null)
            {
                builder.AppendLine(descriptionAttr.Description);
            }

            return builder.ToString();
        }

        private void WriteActionFooter(Command command, StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine("To get help for actions");
            var helpActionName = GetHelpActionName(command);

            if (command.IsRoot())
            {
                builder.AppendFormat("\t{0} <action>", helpActionName)
                    .AppendLine();
            }
            else
            {
                var path = GetFullCommandPath(command).Skip(1).Join(" ");
                builder.AppendFormat("\t{0} {1} <action>", path, helpActionName)
                    .AppendLine();
            }
        }

        #endregion

        #region subcommands

        private void WriteSubCommandsHelp(Command command, StringBuilder builder)
        {
            builder
                .AppendLine()
                .AppendLine()
                .AppendLine("SUB COMMANDS");

            foreach (var subCommand in command.SubCommands.Values)
            {
                var descriptions = GetDescription(subCommand)
                    .Paginate(this.DescriptionWidth)
                    .ToArray()
                    ;

                var first = descriptions.FirstOrDefault() ?? "";
                var theRest = descriptions.Skip(1);
                var spacer = new string(' ', this.SpacerWidth);
                var name = subCommand.Name.PadRight(this.IdentifierWidth);
                var firstLine = $"{name}{spacer}{first}";

                builder.AppendLine(firstLine);

                foreach (var description in theRest)
                {
                    spacer = new string(' ', this.IdentifierWidth + this.SpacerWidth);
                    var line = $"{spacer}{description}";
                    builder.AppendLine(line);
                }
            }

        }

        private void WriteSubCommandsHelpFooter(Command command, StringBuilder builder)
        {
            builder.AppendLine("To get help for subcommands");
            var helpActionName = GetHelpActionName(command);

            if (command.IsRoot())
            {
                builder.AppendFormat("\t{0} <subcommand>", helpActionName);
            }
            else
            {
                var fullPath = GetFullCommandPath(command).Skip(1); // remove the root command from the path.
                var path = string.Join(" ", fullPath);
                builder.AppendFormat("\t{0} {1} <subcommand>", path, helpActionName);
            }
        }

        private static string GetHelpActionName(Command command)
        {
            var helpActionName = command.Conventions.GetActionName(command.GetType().GetMethod("Help"));
            return helpActionName;
        }

        #endregion
    }
}
