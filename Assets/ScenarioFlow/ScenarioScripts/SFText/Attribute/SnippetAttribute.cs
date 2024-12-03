using System;

namespace ScenarioFlow.Scripts.SFText
{
    /// <summary>
    /// Attach a snippet used when inserting arguments for a command. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =true)]
    public class SnippetAttribute : Attribute
    {
        public string Text { get; }

        public SnippetAttribute(string text)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}