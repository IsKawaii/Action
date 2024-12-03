using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using ScenarioFlow.Tasks;
using System;
using System.Threading;

public class ScenarioTaskConsumer : IReflectable
{
	private readonly IScenarioTaskStorage scenarioTaskStorage;

	public ScenarioTaskConsumer(IScenarioTaskStorage scenarioTaskStorage)
	{
		this.scenarioTaskStorage = scenarioTaskStorage ?? throw new ArgumentNullException(nameof(scenarioTaskStorage));
	}

	[CommandMethod("accept task async")]
	[Category("Task")]
	[Description("Wait for the completion of the tasks with the general token code.")]
	[Snippet("Wait for the completion of {${1:task name}}.")]
	public UniTask AcceptTaskAsync(string tokenCode, CancellationToken cancellationToken)
	{
		return scenarioTaskStorage.AcceptAsync(tokenCode, cancellationToken);
	}

	[CommandMethod("cancel task")]
	[Category("Task")]
	[Description("Cancel the tasks with the general token code.")]
	[Snippet("Cancel task {${1:task name}}.")]
	public void CancelTask(string tokenCode)
	{
		scenarioTaskStorage.Cancel(tokenCode);
	}
}