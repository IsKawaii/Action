using System;
using System.Collections.Generic;
using System.Linq;

namespace ScenarioFlow
{
    /// <summary>
    /// A object that preserves information about commands to call and labels bound to the commands.
    /// </summary>
    public class ScenarioBook
    {
        private readonly Func<object>[] commandMethods;
        private readonly Dictionary<string, int> labelDictionary;
        /// <summary>
        /// Current index of the scenario book. The command with this index will be invoked when the 'Read' method is called.
        /// </summary>
        public int CurrentIndex { get; private set; } = 0;
        /// <summary>
        /// The length of the scenario book.
        /// </summary>
        public int Length => commandMethods.Length;

        public ScenarioBook(IEnumerable<Func<object>> commandMethods, IReadOnlyDictionary<string, int> labelDictionary)
        {
            this.commandMethods = commandMethods.ToArray();

            this.labelDictionary = labelDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

		/// <summary>
		/// Get the index bound to the label with the name.
		/// </summary>
		/// <param name="labelName"></param>
		/// <returns></returns>
		public int LabelIndex(string labelName)
		{
			return labelDictionary.TryGetValue(labelName, out int result) ? result : -1;
		}

		/// <summary>
		/// Changes the current index of to the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ScenarioBook OpenTo(int index)
        {
            CurrentIndex = index;
            
            return this;
        }

        /// <summary>
        /// Invoke the command bound to the current index.
        /// </summary>
        /// <returns></returns>
        public object Read()
        {
            if (CurrentIndex < 0 || Length <= CurrentIndex)
            {
                //Do nothing
                return new object();
            }
            else
            {
                return commandMethods[CurrentIndex].Invoke();
            }
        }
    }
}