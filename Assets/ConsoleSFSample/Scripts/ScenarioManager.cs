using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using ScenarioFlow;
using ScenarioFlow.Scripts;
using ScenarioFlow.Tasks;
using System;
using UnityEngine;

namespace ConsoleSFSample
{
	public class ScenarioManager : MonoBehaviour
    {
        public GameObject ReadingObject;
        public TextsFolder textDataHolder;

        [SerializeField]
        private ScenarioScript scenarioScript;
        private ScenarioScript[] texts;

        private void Start()
        {
            //ReadScenarioBook();

            //テキストを配列から取得
            if (textDataHolder != null)
            {
                texts = textDataHolder.ReadingObjects;
            }
            else
            {
                Debug.LogError("TextDataHolderが設定されていません！");
            }

            /*スクリプトから全部取得するならこっち
            GameObject textFolderObject = GameObject.Find("TextFolder");
            TextsFolder textsFolder = textFolderObject.GetComponent<TextsFolder>();
            ScenarioScript[] textsArray = textsFolder.ReadingObjects;
            foreach (ScenarioScript obj in textsArray)
            {
                Debug.Log(obj.name);
            }
            */

        }

        public void ReadScenarioBook(int TextNumber)
        {
            ConsoleDialogueWriter consoleDialogueWriter = gameObject.GetComponent<ConsoleDialogueWriter>();
            if (consoleDialogueWriter == null)
            {
                consoleDialogueWriter = gameObject.AddComponent<ConsoleDialogueWriter>();
            }

            //Build ScenarioBookReader ------
            //EnterKeyNotifier enterKeyNotifier = new EnterKeyNotifier();
            IKeyNotifier iKeyNotifier = new IKeyNotifier();

            //INextNotifier nextNotifier = enterKeyNotifier;
            INextNotifier nextNotifier = iKeyNotifier;
            //ICancellationNotifier cancellationNotifier = enterKeyNotifier;

            MouseNotifier mouseNotifier = new MouseNotifier();

            //INextNotifier nextNotifier = mouseNotifier;
            ICancellationNotifier cancellationNotifier = mouseNotifier;
            ScenarioTaskExecutor scenarioTaskExecutor = new ScenarioTaskExecutor(nextNotifier, cancellationNotifier);

            IDisposable disposable = scenarioTaskExecutor;
            disposable.AddTo(this.GetCancellationTokenOnDestroy());

            IScenarioTaskExecutor scenarioTaskExecutorInterface = scenarioTaskExecutor;
            ScenarioBookReader scenarioBookReader = new ScenarioBookReader(scenarioTaskExecutorInterface);
            //------

            //Build ScenarioBookPublisher ------
            //Register commands and decoders
            ILabelOpener labelOpener = scenarioBookReader;
            ICancellationTokenDecoder cancellationTokenDecoder = scenarioTaskExecutor;
            IScenarioTaskStorage scenarioTaskStorage = scenarioTaskExecutor;
            ScenarioBookPublisher scenarioBookPublisher = new ScenarioBookPublisher(
                new IReflectable[]
                {
                    new BranchMaker(labelOpener),
                    new CancellationTokenDecoder(cancellationTokenDecoder),
                    new ColorDecoder(),
                    //new ConsoleDialogueWriter(),
                    consoleDialogueWriter,
                    new DelayMaker(),
                    new PrimitiveDecoder(),
                    new MessageLogger(),
                    new BoolDecoder(),
                    new ScenarioTaskConsumer(scenarioTaskStorage),
                });
            //------

			//The skip mode is switched with the S key
			ISkipActivator skipActivator = scenarioTaskExecutor;
			skipActivator.Duration = 0.05f;
			UniTaskAsyncEnumerable.EveryUpdate()
				.Select(_ => Input.GetKeyDown(KeyCode.S))
				.Where(x => x)
				.ForEachAsync(_ =>
				{
					skipActivator.IsActive = !skipActivator.IsActive;
					Debug.Log($"Skip Mode: {skipActivator.IsActive}");
				}, cancellationToken: this.GetCancellationTokenOnDestroy()).Forget();


            //Start running a script ------
			//Convert the scenario script to a scenario book
			IScenarioBookPublisher scenarioBookPublisherInterface = scenarioBookPublisher;
            //IScenarioScript scenarioScriptInterface = scenarioScript;
            IScenarioScript scenarioScriptInterface = texts[TextNumber];
            ScenarioBook scenarioBook = scenarioBookPublisherInterface.Publish(scenarioScriptInterface);

            //Start to read the scenario book
            IScenarioBookReader scenarioBookReaderInterface = scenarioBookReader;
            scenarioBookReaderInterface.ReadAsync(scenarioBook, this.GetCancellationTokenOnDestroy()).Forget();
            //------
        }

        public void ActivateReader()
        {
            gameObject.SetActive(true);
        }
    }
}