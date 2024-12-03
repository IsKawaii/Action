using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow.Tasks;
using System.Threading;
using UnityEngine;

public class MouseNotifier : INextNotifier, ICancellationNotifier
{
	public UniTask NotifyNextAsync(CancellationToken cancellationToken)
	{
		return WaitUntilMouseDown(cancellationToken);
	}

	public UniTask NotifyCancellationAsync(CancellationToken cancellationToken)
	{
		return WaitUntilMouseDown(cancellationToken);
	}

	private UniTask WaitUntilMouseDown(CancellationToken cancellationToken)
	{
		return UniTaskAsyncEnumerable.EveryUpdate()
			.Select(_ => Input.GetMouseButtonDown(0))
			.Where(x => x)
			.FirstOrDefaultAsync(cancellationToken: cancellationToken);
	}
}