using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using ScenarioFlow;
using ScenarioFlow.Scripts.SFText;
using ScenarioFlow.Tasks;
using System;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ConsoleSFSample
{
    /// <summary>
    /// Provides functions to make scenario branches.
    /// </summary>
    public class BranchMaker : IReflectable
    {
        private readonly ILabelOpener labelOpener;

        public BranchMaker(ILabelOpener labelOpener)
        {
            this.labelOpener = labelOpener ?? throw new ArgumentNullException(nameof(labelOpener));
        }

        [CommandMethod("jump to label")]
        [Category("Branch")]
        [Description("Jump to the specified label.")]
        [Snippet("Jump to {${1:label}}.")]
        public void JumpLabel(string label)
        {
            labelOpener.OpenLabel(label);
        }

		[CommandMethod("branch on 2 selections async")]
		[Category("Branch")]
		[Description("Present 2 selections to the player, and branch based on the selection.")]
		[Snippet("Selection 1: {${1:Selection1}} ")]
		[Snippet("- Jump to {${2:Label1}}")]
		[Snippet("Selection 2: {${3:Selection2}}")]
		[Snippet("- Jump to {${4:Label2}}")]
		public async UniTask BranchBasedOnTwoSelectionsAsync(string selection1, string label1, string selection2, string label2, CancellationToken cancellationToken)
		{
			var builder = new StringBuilder();
			builder.AppendLine("Select your answer with the number keys:");
			builder.AppendLine($"1 - {selection1}, 2 - {selection2}");
			Debug.Log(builder.ToString());

			var answer = await UniTask.WhenAny(
				WaitUntilKeyPressedAsync(KeyCode.Alpha1, cancellationToken),
				WaitUntilKeyPressedAsync(KeyCode.Alpha2, cancellationToken));
			var label = answer == 0 ? label1 : label2;
			labelOpener.OpenLabel(label);
		}

		private UniTask WaitUntilKeyPressedAsync(KeyCode keyCode, CancellationToken cancellationToken)
		{
			return UniTaskAsyncEnumerable.EveryUpdate()
				.Select(_ => Input.GetKeyDown(keyCode))
				.Where(x => x)
				.FirstOrDefaultAsync(cancellationToken: cancellationToken);
		}
	}
}