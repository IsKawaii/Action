using System.Threading;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Provides functions to create a decoder for the 'CancellationToken' type.
    /// </summary>
    public interface ICancellationTokenDecoder
    {
        /// <summary>
        /// Returns a cancellation token with the specific characteristic depending on the token code.
        /// </summary>
        /// <param name="tokenCode">A token code to specify the characteristic of the next async command.</param>
        /// <returns></returns>
        CancellationToken Decode(string tokenCode);
    }
}
