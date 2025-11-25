using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Character.Repository;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.Ingame.DataAsset;
using Cryptos.Runtime.InfraStructure.Ingame.Utility;
using Cryptos.Runtime.Presenter.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.Character.Enemy;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.System;
using Cryptos.Runtime.UI.Ingame.Manager;
using Cryptos.Runtime.UI.System.Audio;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.System;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.Ingame.Sequence
{
    /// <summary>
    /// インゲームの開始シーケンスを管理するクラス。
    /// </summary>
    [Serializable]
    public class IngameStartSequence : IGameInstaller, IDisposable
    {
        /// <summary>
        /// このインスタンスが破棄されるときに呼び出されます。
        /// </summary>
        public void Dispose()
        {
            ServiceLocator.DestroyInstance<CardUseCase>();
        }

        [SerializeField, Tooltip("プレイヤーデータ")]
        private CharacterData _symphonyData;
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        [SerializeField, Tooltip("レベルアップデータ")]
        private LevelUpgradeData _levelUpgradeData;

        [Space]
        [SerializeField, Tooltip("戦闘パイプライン")]
        private CombatPipelineAsset _combatPipelineAsset;

        [Header("テストコード")]
        [SerializeField, Tooltip("テスト用のカードデータ")]
        private CardDataAsset[] _cardDatas;
        [SerializeField, Min(1), Tooltip("テスト用に生成するカードの数")]
        private int _cardAmount = 3;

        [Space]
        [SerializeField, Tooltip("ウェーブのエンティティ配列")]
        private WaveEntity[] _waveEntities;

        [Space]
        [SerializeField, Tooltip("ウェーブ移動速度")]
        private float _waveMoveSpeed = 3;

        private IngameUIManager _gameUIManager;
        private InputBuffer _inputBuffer;

        /// <summary>
        /// ゲームを初期化します。
        /// </summary>
        public async void GameInitialize()
        {
            TentativeCharacterData symphonyData = new(_symphonyData);

            CharacterInitializer.CharacterInitializationData charaInitData =
                CharacterInitializer.Initialize(symphonyData);

            CardInitializer.CardInitializationData cardInitData =
                CardInitializer.Initialize(_wordDataBase,
                charaInitData.Symphony, charaInitData.EnemyRepository);

            LevelUseCase levelUseCase =
                new LevelUseCase(_levelUpgradeData, LevelUpAsync);

            InputBuffer inputBuffer =
                await ServiceLocator.GetInstanceAsync<InputBuffer>();
            PlayerPathContainer playerPathContainer =
                await ServiceLocator.GetInstanceAsync<PlayerPathContainer>();
            IngameUIManager ingameUIManager =
                await ServiceLocator.GetInstanceAsync<IngameUIManager>();
            SymphonyPresenter symphonyPresenter =
                await ServiceLocator.GetInstanceAsync<SymphonyPresenter>();
            EnemyPresenter enemyPresenter =
                await ServiceLocator.GetInstanceAsync<EnemyPresenter>();
            BGMManager bgmPlayer =
                await ServiceLocator.GetInstanceAsync<BGMManager>();

            await InitializeUtility.WaitInitialize(ingameUIManager);

            CardPresenter cardPresenter = new CardPresenter(cardInitData.CardUseCase, ingameUIManager);
            WavePathPresenter wavePathPresenter = new(playerPathContainer, symphonyPresenter, _waveMoveSpeed);

            symphonyPresenter.Init(cardInitData.CardUseCase, charaInitData.Symphony);

            //SymphonyのUI表示
            ingameUIManager.CreateHealthBar(
                new(charaInitData.Symphony,
                symphonyPresenter.transform,
                symphonyPresenter.destroyCancellationToken));
            charaInitData.Symphony.OnTakedDamage += c =>
                ingameUIManager.ShowDamageText(new(c), symphonyPresenter.transform.position);

            enemyPresenter.Init(charaInitData.EnemyRepository, symphonyPresenter,
                new(_combatPipelineAsset.CombatHandler));

            enemyPresenter.OnCreatedEnemyModel += HandleEnemyCreated;

            WaveSystemPresenter waveSystem = new(_waveEntities,
                wavePathPresenter,
                symphonyPresenter, charaInitData.EnemyRepository,
                levelUseCase, symphonyData,
                bgmPlayer);

            // ウェーブ開始時に入力受付を開始、ウェーブクリア時に入力受付を停止する。
            waveSystem.OnWaveStarted += () =>
                {
                    inputBuffer.OnAlphabetKeyPressed += cardInitData.CardUseCase.InputCharToDeck;
                    inputBuffer.OnAlphabetKeyPressed += ingameUIManager.CardInputChar;
                };
            waveSystem.OnWaveCleared += () =>
            {
                inputBuffer.OnAlphabetKeyPressed -= cardInitData.CardUseCase.InputCharToDeck;
                inputBuffer.OnAlphabetKeyPressed -= ingameUIManager.CardInputChar;
            };
            waveSystem.OnAllWaveEnded += GoToOutGameScene;

            _gameUIManager = ingameUIManager;
            _inputBuffer = inputBuffer;

            TestCardSpawn(cardInitData.CardUseCase);
            waveSystem.GameStart();
        }

        /// <summary>
        /// レベルアップ時の非同期処理。
        /// </summary>
        /// <param name="nodes">レベルアップ候補のノード。</param>
        /// <returns>選択されたレベルアップノード。</returns>
        private async Task<LevelUpgradeNode> LevelUpAsync(LevelUpgradeNode[] nodes)
        {
            LevelUpgradeNodeViewModel[] levelUpgradeNodes
                = nodes.Select(n => new LevelUpgradeNodeViewModel(n)).ToArray();

            Debug.Log($"候補カード {string.Join(", ", levelUpgradeNodes.Select(n => n.NodeName))}");

            // ウィンドウを出現させて待機。
            _gameUIManager.OpenLevelUpgradeWindow(levelUpgradeNodes);
            _inputBuffer.OnAlphabetKeyPressed += _gameUIManager.OnLevelUpgradeInputChar;

            LevelUpgradeNodeViewModel selectedNodeVM = default;
            await SymphonyTask.WaitUntil(
                () => _gameUIManager.TryGetSelectedLevelUpgradeNode(out selectedNodeVM));

            LevelUpgradeNode selectedNode = selectedNodeVM.LevelUpgradeNode;

            Debug.Log($"レベルアップカードを選択しました。{selectedNode}");

            _inputBuffer.OnAlphabetKeyPressed -= _gameUIManager.OnLevelUpgradeInputChar;
            _gameUIManager.CloseLevelUpgradeWindow();

            return selectedNode;
        }

        /// <summary>
        /// テスト用にカードを生成します。
        /// </summary>
        /// <param name="cardUseCase">カードのユースケース。</param>
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
                CardDataAsset cardDataAsset = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                CardData cardData = GetCardData(cardDataAsset);
                CardEntity instance = cardUseCase.CreateCard(cardData);

                instance.OnComplete += RandomDraw;
            }
        }

        /// <summary>
        ///     カードデータをインスタンス化します。
        /// </summary>
        /// <param name="dataAsset"></param>
        /// <returns></returns>
        private CardData GetCardData(CardDataAsset dataAsset)
        {
            return dataAsset.CreateCardData(_combatPipelineAsset);
        }

        private void HandleEnemyCreated(CharacterEntity enemy, EnemyModelPresenter model)
        {
            enemy.OnTakedDamage += DamageHealthChange;

            void DamageHealthChange(CombatContext cc)
            {
                _gameUIManager.ShowDamageText(new(cc), model.transform.position);
            }

            _gameUIManager.CreateHealthBar(new(enemy, model.transform, model.destroyCancellationToken));
        }

        private async void GoToOutGameScene()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;
            
            await SceneLoader.UnloadScene(SceneListEnum.Stage.ToString());
            await SceneLoader.UnloadScene(SceneListEnum.Ingame.ToString());

            await SceneLoader.LoadScene(SceneListEnum.Outgame.ToString());
            await SceneLoader.LoadScene(SceneListEnum.Stage.ToString());
            SceneLoader.SetActiveScene(SceneListEnum.Stage.ToString());
        }

        private bool _isTransitioning;
    }
}
