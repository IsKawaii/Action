using Cysharp.Threading.Tasks;
using System.Threading;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Manages executions of async commands.
    /// </summary>
    public interface IScenarioTaskExecutor
    {
        /// <summary>
        /// Executes an async command as a scenario task.
        /// </summary>
        /// <param name="scenarioTask">The async command running.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        UniTask ExecuteAsync(UniTask scenarioTask, CancellationToken cancellationToken);
    }
}