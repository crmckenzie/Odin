using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Odin
{
    [DebuggerDisplay("HasNamingConflict: {nameof(HasNamingConflict)}")]
    internal class ParameterAnalysis
    {
        public Action Action { get; set; }
        public ActionParameter Parameter { get; set; }
        public SharedParameter SharedParameter { get; set; }
        public bool HasNamingConflict { get; set; }
        public IEnumerable<string> NamingConflictsForAliases { get; set; }

        public string[] GetNamingConflictsMessages()
        {
            var conflicts = SharedParameter
                    .GetNamingConflictForAliases(Parameter)
                    .Select(alias =>
                        $"The alias '{alias}' for shared parameter '{SharedParameter.LongOptionName}' is duplicated for parameter '{Parameter.LongOptionName}' on action '{Action.Name}'.")
                    .ToArray()
                ;
            return conflicts;
        }
    }
}