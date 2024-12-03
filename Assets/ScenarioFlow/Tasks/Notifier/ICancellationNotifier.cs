using Cysharp.Threading.Tasks;
using System.Threading;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Triggers the cancellation-instruction.
    /// </summary>
    public interface ICancellationNotifier
    {
        /// <summary>
        /// Triggers the cancellation-instruction after the completion of this task.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        UniTask NotifyCancellationAsync(CancellationToken cancellationToken);
    }
}