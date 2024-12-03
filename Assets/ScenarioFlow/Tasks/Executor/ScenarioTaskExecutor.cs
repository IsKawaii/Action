using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using static ScenarioFlow.Tasks.SpecialTokenCodes;

namespace ScenarioFlow.Tasks
{
    /// <summary>
    /// Manages executions of async commands.
    /// </summary>
    public class ScenarioTaskExecutor : IScenarioTaskExecutor, IScenarioTaskStorage, ICancellationTokenDecoder, ITokenCodeGetter, ISkipActivator, IDisposable
    {
        //Token code specified for next scenario task
        public string TokenCode { get; private set; } = null;

        //Is skip mode active
        public bool IsActive { get; set; } = false;
        //Waiting time when skip mode is active
        public float Duration { get; set; } = 0.0f;

        //The trigger to move on to the next task
        private readonly INextNotifier nextNotifier;
        //The trigger to cancel the task running
        private readonly ICancellationNotifier cancellationNotifier;

        //When a serial task is canceled, continue to cancel the following tasks until the plain or fluent task is canceled
        private bool isSerialCancellationActive = false;
        //Parallel tasks can't be canceled until the next plain or fluent task is executed
        private bool isParallelCancellationReady = false;
        //Pass this token to the cancellation notifier mainly
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //Pass this token to the next notifier mainly
        private CancellationTokenSource nextTokenSource = new CancellationTokenSource();
        //To return tokens for general token codes
        private readonly Dictionary<string, CancellationTokenSource> generalTokenSourceDictionary = new Dictionary<string, CancellationTokenSource>();
        //To store tasks bound to general token codes
        private readonly Dictionary<string, List<UniTask>> generalTaskListDictionary = new Dictionary<string, List<UniTask>>();
        //To store tasks bound to parallel token code
        private readonly List<UniTask> paralTaskList = new List<UniTask>();

        //Has this object been disposed
        private bool isDisposed = false;

        public ScenarioTaskExecutor(INextNotifier nextNotifier, ICancellationNotifier cancellationNotifier)
        {
            this.nextNotifier = nextNotifier ?? throw new ArgumentNullException(nameof(nextNotifier));
            this.cancellationNotifier = cancellationNotifier ?? throw new ArgumentNullException(nameof(cancellationNotifier));
        }

        //Execute Unitask as a scenario task
        public async UniTask ExecuteAsync(UniTask scenarioTask, CancellationToken cancellationToken)
        {
            //Throw an exception if this object has been disposed already
            ThrowIfDisposed();
            //Get the specified token code
            var tokenCode = TokenCode;
            try
            {
                //Special token code
                if (IsSpecial(tokenCode))
                {
                    //Parallel task
                    if (IsParallel(tokenCode))
                    {
                        //Add the task to the list
                        paralTaskList.Add(scenarioTask);
                    }
                    //Plain, fluent, or serial task
                    else
                    {
                        try
                        {
                            //Parallel tasks can be canceled
                            isParallelCancellationReady = true;
                            //Wait for the passed task completion
                            await scenarioTask.AttachExternalCancellation(cancellationToken);
                            //Wait for the parallel tasks completion if necessary
                            if (paralTaskList.Count > 0)
                            {
                                await UniTask.WhenAll(paralTaskList).AttachExternalCancellation(cancellationToken);
                            }
                        }
                        //The task can be canceled
                        catch (OperationCanceledException)
                        {

                        }
                        finally
                        {
                            //Clear the parallel task list
                            paralTaskList.Clear();
                            //Plain or fluent task
                            if (IsPlain(tokenCode) || IsFluent(tokenCode))
                            {
                                //Re-create the cancellation token source
                                RecreateCancellationTokenSource();
                                //Inactivate the serial cancellation mode
                                isSerialCancellationActive = false;
                            }
                        }
                        //Plain task
                        if (IsPlain(tokenCode))
                        {
                            var skipToken = WaitUntilSkipModeActivated(nextTokenSource.Token).ToCancellationToken();
                            try
                            {
                                //Wait for the next notifier completion or the skip mode activated
                                await UniTask.WhenAny(nextNotifier.NotifyNextAsync(skipToken), cancellationToken.ToUniTask().Item1);
                            }
                            catch (OperationCanceledException)
                            {

                            }
                            //Re-create the next token source
                            RecreateNextTokenSource();
                        }
                    }
                }
                //General token code
                else
                {
                    if (!generalTaskListDictionary.ContainsKey(tokenCode))
                    {
                        generalTaskListDictionary.Add(tokenCode, new List<UniTask>());
                    }
                    generalTaskListDictionary[tokenCode].Add(scenarioTask);
                }
            }
            finally
            {
                //If the cancellation is requested
                if (cancellationToken.IsCancellationRequested)
                {
                    //Recreate token sources
                    RecreateCancellationTokenSource();
                    RecreateNextTokenSource();
                    //Throw an exception
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        //Check token code and generate CancellationToken for it
        public CancellationToken Decode(string tokenCode)
        {
            //Throw an exception if this object has been disposed already
            ThrowIfDisposed();
            //Recorde the specified token code
            TokenCode = tokenCode;
            //Special token code
            if (IsSpecial(tokenCode))
            {
                return UniTask.Create(async () =>
                {
                    var token = cancellationTokenSource.Token;
                    //If the serial cancellation is active, cancel the task immediately
                    if (isSerialCancellationActive)
                    {
                        await UniTask.DelayFrame(0, cancellationToken: token);
                        return;
                    }
                    //If the parallel task, wait until the plain or fluent task is executed
                    if (IsParallel(tokenCode))
                    {
                        isParallelCancellationReady = false;
                        await UniTaskAsyncEnumerable.EveryValueChanged(this, x => x.isParallelCancellationReady)
                        .Where(x => x)
                        .FirstOrDefaultAsync(cancellationToken: token);
                    }
                    //The task can be canceled by both the cancellation notifier and the skip mode
                    if (IsStandard(tokenCode))
                    {
                        await UniTask.WhenAny(
                            cancellationNotifier.NotifyCancellationAsync(token),
                            WaitUntilSkipModeActivated(token));
                    }
                    //The task can be canceled only by the skip mode
                    else if (IsForced(tokenCode))
                    {
                        await WaitUntilSkipModeActivated(token);
                    }
                    //Promised task
                    //The task can't be canceled
                    else
                    {
                        await UniTask.Never(token);
                    }
                    //If the serial task is canceled, activate the serial cancellation
                    if (!token.IsCancellationRequested && IsSerial(tokenCode))
                    {
                        isSerialCancellationActive = true;
                    }
                }).ToCancellationToken();
            }
            //General token code
            else
            {
                if (!generalTokenSourceDictionary.ContainsKey(tokenCode))
                {
                    generalTokenSourceDictionary.Add(tokenCode, new CancellationTokenSource());
                }
                return generalTokenSourceDictionary[tokenCode].Token;
            }
        }

        private void RecreateCancellationTokenSource()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void RecreateNextTokenSource()
        {
            nextTokenSource?.Cancel();
            nextTokenSource?.Dispose();
            nextTokenSource = new CancellationTokenSource();
        }

        //Wait until skip mode become active
        private async UniTask WaitUntilSkipModeActivated(CancellationToken cancellationToken)
        {
            if (IsActive)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Duration), cancellationToken: cancellationToken);
            }
            //If the skip mode is inactive initially or it is inactivated while waiting
            if (!IsActive)
            {
                await UniTaskAsyncEnumerable.EveryValueChanged(this, x => x.IsActive)
                    .Where(x => x)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            }
        }

        //Cunsume scenario tasks for which an arbitrary token code is specified
        public async UniTask AcceptAsync(string tokenCode, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            //Check if scenario tasks bound to the token code exist
            if (!generalTaskListDictionary.ContainsKey(tokenCode))
            {
                throw new ArgumentException($"Any scenaio tasks bound to '{tokenCode}' don't exist.");
            }

            try
            {
                await UniTask.WhenAll(generalTaskListDictionary[tokenCode]).AttachExternalCancellation(cancellationToken);
            }
            finally
            {
                Cancel(tokenCode);
            }
        }

        //Cancel scenario tasks for which an arbitrary token code is specified
        public void Cancel(string tokenCode)
        {
            ThrowIfDisposed();

            //Check if scenario tasks bound to the token code exist
            if (!generalTaskListDictionary.ContainsKey(tokenCode))
            {
                throw new ArgumentException($"Any scenaio tasks bound to '{tokenCode}' don't exist.");
            }

            generalTokenSourceDictionary[tokenCode].Cancel();
            generalTokenSourceDictionary[tokenCode].Dispose();
            generalTaskListDictionary.Remove(tokenCode);
            generalTaskListDictionary.Remove(tokenCode);
        }

        //Dispose CancellationTokenSources
        public void Dispose()
        {
            if (!isDisposed)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                nextTokenSource?.Cancel();
                nextTokenSource?.Dispose();
                isDisposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(ScenarioTaskExecutor));
            }
        }
    }
}