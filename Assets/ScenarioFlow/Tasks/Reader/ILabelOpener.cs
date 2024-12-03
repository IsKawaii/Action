namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Provides functions to open labels in the running scenario book.
    /// </summary>
    public interface ILabelOpener
    {
        /// <summary>
        /// Open the label in the running book.
        /// </summary>
        /// <param name="label"></param>
        void OpenLabel(string label);
    }
}