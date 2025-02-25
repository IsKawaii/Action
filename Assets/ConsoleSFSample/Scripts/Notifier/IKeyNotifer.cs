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
	public class IKeyNotifier : INextNotifier, ICancellationNotifier
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
                //.Select(_ => InputManager.instance.GetKeyDown(KeyCode.I))
                //.Select(_ => Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame && InputSystemManager.GetCurrentActionMapName == "Talk")
                .Select(_ =>
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame) || // コントローラのボタン
                (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame))   // キーボードのスペースキー
                && InputSystemManager.GetCurrentActionMapName == "Talk")
                .Where(x => x)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}