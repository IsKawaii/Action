using System;

namespace ScenarioFlow
{
    /// <summary>
    /// A method with this attribute is exported as a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandMethodAttribute: Attribute
    {
        /// <summary>
        /// Command name.
        /// </summary>
        public string Name { get; }

        public CommandMethodAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Command name must not be empty.");
            }
            Name = name;
        }
    }
}