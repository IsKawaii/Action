using Cysharp.Threading.Tasks;
using System.Threading;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Stores scenario tasks with general token codes.
    /// </summary>
    public interface IScenarioTaskStorage
    {
        /// <summary>
        /// Waits for the completion of the scenario tasks with the general token code.
        /// </summary>
        /// <param name="tokenCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        UniTask AcceptAsync(string tokenCode, CancellationToken cancellationToken);

        /// <summary>
        /// Cancels the scenario tasks with the general token code.
        /// </summary>
        /// <param name="tokenCode"></param>
        void Cancel(string tokenCode);
    }
}