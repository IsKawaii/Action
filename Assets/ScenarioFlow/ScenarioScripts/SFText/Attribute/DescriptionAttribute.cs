using System;

namespace ScenarioFlow.Scripts.SFText
{
    /// <summary>
    /// Attach a description to a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =true)]
    public class DescriptionAttribute : Attribute
    {
        public string Text { get; set; }

        public DescriptionAttribute(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.Text = text;
        }
    }
}