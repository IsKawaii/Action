using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using ScenarioFlow.Tasks;
using System;
using System.Threading;

namespace ConsoleSFSample
{
    /// <summary>
    /// Provides a decoder for the 'CancellationToken' type.
    /// </summary>
    public class CancellationTokenDecoder : IReflectable
    {
        private readonly ICancellationTokenDecoder cancellationTokenDecoder;

        public CancellationTokenDecoder(ICancellationTokenDecoder cancellationTokenDecoder)
        {
            this.cancellationTokenDecoder = cancellationTokenDecoder ?? throw new ArgumentNullException(nameof(cancellationTokenDecoder));
        }

        [DecoderMethod]
        [Description("A decoder for the 'CancellationToken' type.")]
        [Description("Returns a cancellation token with the specific characteristic depending on the token code.")]
        public CancellationToken Decode(string tokenCode)
        {
            return cancellationTokenDecoder.Decode(tokenCode);
        }
    }
}