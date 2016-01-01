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

        /// <summary>
        /// Gets the total width of the help window.
        /// </summary>
        public int PageWidth => this.IdentifierWidth + this.SpacerWidth + this.DescriptionWidth;

        /// <summary>
        /// Gets the width of the space allocated for the Identifier.
        /// </summary>
        public int IdentifierWidth { get; set; }

        /// <summary>
        /// Gets the width of the empty space between columns.
        /// </summary>
        public int SpacerWidth { get; set; }

        /// <summary>
        /// Gets the width of indentation space.
        /// </summary>
        public int IndentWidth { get; set; }

        /// <summary>
        /// Gets the width of the Description column.
        /// </summary>
        public int DescriptionWidth { get; set; }

        /// <summary>
        /// Returns the help text for the command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="actionName">If specified, help is scoped to a particular action.</param>
        /// <returns></returns>
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
            var action = command.Actions.Values.FirstOrDefault(row => row.Identifiers.Contains(actionName));
            if (action != null)
            {
                WriteActionHelp(action, builder);
                return;
            }

            var subCommand = command.SubCommands.Values.FirstOrDefault(row => row.Identifiers.Contains(actionName));
            if (subCommand!= null)
            {
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
                builder.Append(defaultDescription);
            }
            else
            {
                builder.Append(attribute.Description);
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
                builder.Append(descriptionAttr.Description);
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
                builder.Append(descriptionAttr.Description);
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
