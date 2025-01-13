using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

namespace ConsoleSFSample
{
    /// <summary>
    /// Provides notifiers that trigger the next/cancellation instruction when the enter key is pressed.
    /// </summary>
	public class SpaceKeyNotifier : INextNotifier, ICancellationNotifier
    {
        public bool OnRead(InputAction.CallbackContext context)
        {
            if (context.performed) 
            {
                return true;          
            }     
            return false;   
        }

        public UniTask NotifyNextAsync(CancellationToken cancellationToken)
        {
            return WaitUntilEnterPressedAsync(cancellationToken);
        }

        public UniTask NotifyCancellationAsync(CancellationToken cancellationToken)
        {
            return WaitUntilEnterPressedAsync(cancellationToken);
        }

        private UniTask WaitUntilEnterPressedAsync(CancellationToken cancellationToken)
        {
            return UniTaskAsyncEnumerable
                .EveryUpdate()
                .Select(_ => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && InputSystemManager.GetCurrentActionMapName == "Talk")
                .Where(x => x)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}