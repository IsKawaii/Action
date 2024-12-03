using System;

namespace ScenarioFlow.Scripts.SFText
{
    /// <summary>
    /// Classify a command according to category.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CategoryAttribute : Attribute
    {
        public string Name { get; }

        public CategoryAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name must not be whitespace.");
            }

            this.Name = name;
        }
    }
}