using Cysharp.Threading.Tasks;
using System.Threading;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Provides functions to read a scenario book automatically.
    /// </summary>
    public interface IScenarioBookReader
    {
        /// <summary>
        /// Start running the scenario book.
        /// </summary>
        /// <param name="scenarioBook"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        UniTask ReadAsync(ScenarioBook scenarioBook, CancellationToken token);
    }
}