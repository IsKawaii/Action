namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Control the settings of the skip mode.
    /// </summary>
    public interface ISkipActivator
    {
        /// <summary>
        /// Whether the skip mode is active.
        /// </summary>
        bool IsActive { get; set; }
        /// <summary>
        /// The number of seconds to wait for before moving on to the next command.
        /// </summary>
        float Duration { get; set; }
    }
}