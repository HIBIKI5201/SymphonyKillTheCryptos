using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.Ingame.Utility;
using Cryptos.Runtime.Presenter.Character.Enemy;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.System;
using Cryptos.Runtime.UI.Ingame;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.System;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.Ingame.Sequence
{
    [Serializable]
    public class IngameStartSequence : IGameInstaller, IDisposable
    {
        public void Dispose()
        {
            ServiceLocator.DestroyInstance<CardUseCase>();
        }

        [SerializeField, Tooltip("プレイヤーデータ")]
        private CharacterData _symphonyData;
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        [SerializeField]
        private LevelUpgradeData _levelUpgradeData;

        [Header("テストコード")]
        [SerializeField]
        private CardData[] _cardDatas;
        [SerializeField, Min(1)]
        private int _cardAmount = 3;

        [Space]
        [SerializeField]
        private WaveEntity[] _waveEntities;

        [SerializeField]
        private LevelUpgradeNode[] _levelUpgradeNodes;

        private IngameUIManager _gameUIManager;

        private InputBuffer _inputBuffer;

        public async void GameInitialize()
        {
            CharacterInitializer.CharacterInitializationData charaInitData =
                CharacterInitializer.Initialize(_symphonyData);

            CardInitializer.CardInitializationData cardInitData =
                CardInitializer.Initialize(_wordDataBase,
                charaInitData.Symphony, charaInitData.EnemyRepository);

            LevelUseCase levelUseCase =
                new LevelUseCase(_levelUpgradeData, LevelUpAsync);

            PlayerPathContainer playerPathContainer = await ServiceLocator.GetInstanceAsync<PlayerPathContainer>();

            IngameUIManager ingameUIManager =
                await ServiceLocator.GetInstanceAsync<IngameUIManager>();
            await InitializeUtility.WaitInitialize(ingameUIManager);
            CardPresenter cardPresenter = new CardPresenter(cardInitData.CardUseCase, ingameUIManager);

            SymphonyPresenter symphonyPresenter =
                await ServiceLocator.GetInstanceAsync<SymphonyPresenter>();
            symphonyPresenter.Init(cardInitData.CardUseCase, playerPathContainer);

            EnemyPresenter enemyPresenter =
                await ServiceLocator.GetInstanceAsync<EnemyPresenter>();
            enemyPresenter.Init(charaInitData.EnemyRepository, symphonyPresenter);

            WaveSystemPresenter waveSystem = new(_waveEntities,
                symphonyPresenter, charaInitData.EnemyRepository,
                levelUseCase);

            InputBuffer inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            // ウェーブ開始時に入力受付を開始、ウェーブクリア時に入力受付を停止する。
            waveSystem.OnWaveStarted += 
                () => inputBuffer.OnAlphabetKeyPressed += cardInitData.CardUseCase.InputCharToDeck;
            waveSystem.OnWaveCleared += 
                () => inputBuffer.OnAlphabetKeyPressed -= cardInitData.CardUseCase.InputCharToDeck;

            TestCardSpawn(cardInitData.CardUseCase);
            waveSystem.GameStart();

            _gameUIManager = ingameUIManager;
            _inputBuffer = inputBuffer;

            SymphonyFrameWork.Debugger.SymphonyDebugHUD.AddText($"screen time{Time.time}");
        }

        private async Task<LevelUpgradeNode> LevelUpAsync(LevelUpgradeNode[] nodes)
        {
            LevelUpgradeNodeViewModel[] levelUpgradeNodes = _levelUpgradeNodes
                .Select(n => new LevelUpgradeNodeViewModel(n)).ToArray();

            CancellationTokenSource cts = new();

            _inputBuffer.OnAlphabetKeyPressed += _gameUIManager.OnInutChar;
            _gameUIManager.OpenLevelUpgradeWindow(levelUpgradeNodes, cts);

            Debug.Log($"候補カード {string.Join(", ", nodes.Select(n => n.NodeName))}");

            await SymphonyTask.WaitUntil(() => cts.IsCancellationRequested);

            LevelUpgradeNode selectedNode = nodes[UnityEngine.Random.Range(0, nodes.Length)];

            Debug.Log($"レベルアップカードを選択しました。{selectedNode}");
            _inputBuffer.OnAlphabetKeyPressed -= _gameUIManager.OnInutChar;
            _gameUIManager.CloseLevelUpgradeWindow();

            return selectedNode;
        }

        private void TestCardSpawn(CardUseCase cardUseCase)
        {
            if (_cardDatas == null || _cardDatas.Length == 0)
            {
                Debug.LogWarning("カードデータが設定されていません。");
                return;
            }

            for (int i = 0; i < _cardAmount; i++)
            {
                RandomDraw();
            }

            void RandomDraw()
            {
                Debug.Log("draw");
                var cardData = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                var instance = cardUseCase.CreateCard(cardData);

                instance.OnComplete += RandomDraw;
            }
        }
    }
}
