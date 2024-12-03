using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Provides functions to read a scenario book automatically.
    /// </summary>
    public class ScenarioBookReader : IScenarioBookReader, ILabelOpener
    {
        private readonly IScenarioTaskExecutor scenarioTaskExecutor;

        private ScenarioBook currentScenarioBook;

        public ScenarioBookReader(IScenarioTaskExecutor scenarioTaskExecutor)
        {
            this.scenarioTaskExecutor = scenarioTaskExecutor ?? throw new ArgumentNullException(nameof(scenarioTaskExecutor));
        }

        public async UniTask ReadAsync(ScenarioBook scenarioBook, CancellationToken cancellationToken)
        {
            if (currentScenarioBook != null)
            {
                throw new InvalidOperationException("Another scenario book is being read.");
            }
            if (scenarioBook == null)
            {
                throw new ArgumentNullException(nameof(scenarioBook));
            }
            if (scenarioBook.Length == 0)
            {
                return;
            }

            currentScenarioBook = scenarioBook;
            try
            {
                //Read the scenario book to the end
                while (true)
                {
                    //Check if cancellation token is canceled
                    cancellationToken.ThrowIfCancellationRequested();
                    //Read
                    var scenarioObject = scenarioBook.Read();
                    //If the scenario object is UniTask, then await it
                    if (scenarioObject is UniTask scenarioTask)
                    {
                        await scenarioTaskExecutor.ExecuteAsync(scenarioTask, cancellationToken);
                    }
                    //Check if scenario book have any exection
                    if (scenarioBook.Remain())
                    {
                        scenarioBook.Flip();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                currentScenarioBook = null;
            }
        }

        public void OpenLabel(string label)
        {
            if (currentScenarioBook == null)
            {
                throw new InvalidOperationException("No scenario book is being read.");
            }
            if (currentScenarioBook.LabelIndex(label) == -1)
            {
                throw new ArgumentException($"Label '{label}' does not exist.");
            }
            currentScenarioBook.OpenTo(currentScenarioBook.LabelIndex(label) - 1);
        }
    }
}
