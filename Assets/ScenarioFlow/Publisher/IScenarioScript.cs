using System.Collections.Generic;

namespace ScenarioFlow
{
    /// <summary>
    /// A material to create scenario books.
    /// </summary>
    public interface IScenarioScript
    {
        /// <summary>
        /// Instruction codes to call commands.
        /// </summary>
        IEnumerable<IEnumerable<string>> Lines { get; }
        /// <summary>
        /// Labels bound to instruction codes.
        /// </summary>
        IReadOnlyDictionary<string, int> LabelDictionary { get; }
    }
}