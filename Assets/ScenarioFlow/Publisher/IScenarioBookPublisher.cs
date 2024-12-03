namespace ScenarioFlow
{
    /// <summary>
    /// Provides functions to create a new scenario book from a scenario script.
    /// </summary>
    public interface IScenarioBookPublisher
    {
        /// <summary>
        /// Creates a new scenario book from the scenario script.
        /// </summary>
        /// <param name="scenarioScript"></param>
        /// <returns></returns>
        ScenarioBook Publish(IScenarioScript scenarioScript);
    }
}