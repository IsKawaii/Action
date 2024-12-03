using System;

namespace ScenarioFlow.Scripts.SFText
{
    /// <summary>
    /// Attach a snippet used when inserting extra arguments for a dialogue command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DialogueSnippetAttribute : Attribute
    {
        public string Text { get; }

        public DialogueSnippetAttribute(string text)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }
}