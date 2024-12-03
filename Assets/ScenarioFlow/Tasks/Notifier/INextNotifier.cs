using Cysharp.Threading.Tasks;
using System.Threading;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Triggers the next-instruction.
    /// </summary>
    public interface INextNotifier
    {
        /// <summary>
        /// Triggers the next-instruction after the completion of this task.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        UniTask NotifyNextAsync(CancellationToken cancellationToken);
    }
}