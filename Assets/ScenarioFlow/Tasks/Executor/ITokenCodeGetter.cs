namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Provides token codes specified.
    /// </summary>
    public interface ITokenCodeGetter
    {
        /// <summary>
        /// The token code that is currently specified.
        /// </summary>
        string TokenCode { get; }
    }
}