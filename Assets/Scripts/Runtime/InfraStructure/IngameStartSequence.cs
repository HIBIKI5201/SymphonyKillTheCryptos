using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.Ingame.Sequence
{
    /// <summary>
    /// インゲームの開始シーケンスを管理するクラス。
    /// DIとオブジェクトの生成、関連付けを担当します。
    /// </summary>
    [Serializable]
    public class IngameStartSequence : IGameInstaller, IDisposable
    {
        [SerializeField, Tooltip("プレイヤーデータ")]
        private CharacterData _symphonyData;
        [SerializeField, Tooltip("コンボデータ")]
        private ComboDataAsset _comboDataAsset;
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
        
        /// <summary>
        /// このインスタンスが破棄されるときに呼び出されます。
        /// </summary>
        public void Dispose()
        {
            ServiceLocator.DestroyInstance<CardUseCase>();
        }

        /// <summary>
        /// ゲームを初期化します。
        /// </summary>
        public async ValueTask GameInitialize()
        {
            List<IDisposable> disposables = new();

            // 初期化とインスタンス取得。
            TentativeCharacterData symphonyTentativeData = new(_symphonyData);
            SymphonyData symphonyData = new(symphonyTentativeData, _comboDataAsset.GetData());
            ComboEntity comboEntity = new(symphonyData);
            var charaInitData = CharacterInitializer.Initialize(symphonyTentativeData);
            var cardInitData = CardInitializer.Initialize(_wordDataBase, comboEntity, charaInitData.Symphony, charaInitData.EnemyRepository);

            LevelUseCase levelUseCase = new(_levelUpgradeData, symphonyTentativeData);
            ServiceLocator.RegisterInstance(levelUseCase);

            WaveUseCase waveUseCase = new(_waveEntities);
            ServiceLocator.RegisterInstance(waveUseCase);

            var inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            var playerPathContainer = await ServiceLocator.GetInstanceAsync<PlayerPathContainer>();
            var ingameUIManager = await ServiceLocator.GetInstanceAsync<IngameUIManager>();
            var symphonyPresenter = await ServiceLocator.GetInstanceAsync<SymphonyPresenter>();
            var enemyPresenter = await ServiceLocator.GetInstanceAsync<EnemyPresenter>();
            var bgmPlayer = await ServiceLocator.GetInstanceAsync<BGMManager>();

            _gameUIManager = ingameUIManager;

            // PresenterとUseCaseのインスタンス生成とDI注入。

            // InputPresenterを作成。
            InputPresenter inputPresenter = 
                new(inputBuffer, cardInitData.CardUseCase, ingameUIManager);

            // WaveControlUseCaseを作成。
            WaveControlUseCase waveControlUseCase = new(charaInitData.EnemyRepository);

            // WaveSystemPresenterを作成。
            WavePathPresenter wavePathPresenter = 
                new(playerPathContainer, symphonyPresenter, _waveMoveSpeed);
            WaveSystemPresenter waveSystemPresenter = new(
                waveUseCase,
                wavePathPresenter,
                symphonyPresenter,
                bgmPlayer,
                inputPresenter,
                waveControlUseCase
            );

            CardExecutionUseCase cardExecutionUseCase = new(
                cardInitData.CardUseCase,
                symphonyPresenter
            );
            disposables.Add(cardExecutionUseCase);

            // レベルアップ時のコールバックを定義。
            // Presenter層にレベルアップ時のUI連携処理を委譲。
            // ILevelUpUIManager として ingameUIManager をDI注入。
            LevelUpPresenter levelUpPresenter = new(levelUseCase, ingameUIManager as ILevelUpUIManager);

            // レベルアップ時のコールバックを定義。
            Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> levelUpSelectCallback = 
                levelUpPresenter.HandleLevelUpSelectAsync;

            // InGameLoopUseCaseを作成し、依存を注入。
            InGameLoopUseCase inGameLoopUseCase = new(
                cardInitData.CardUseCase,
                levelUseCase,
                waveUseCase,
                levelUpSelectCallback,
                symphonyPresenter,
                waveSystemPresenter,
                inputPresenter,
                GoToOutGameScene,
                disposables.ToArray()
            );

            await InitializeUtility.WaitInitialize(_gameUIManager);

            // その他の初期化を行う。
            CardPresenter cardPresenter = new(cardInitData.CardUseCase,cardExecutionUseCase,  ingameUIManager);
            symphonyPresenter.Init(charaInitData.Symphony, symphonyData, cardExecutionUseCase, comboEntity);
            ingameUIManager.CreateHealthBar(new(charaInitData.Symphony, symphonyPresenter.transform, symphonyPresenter.destroyCancellationToken));
            charaInitData.Symphony.OnTakedDamage += c => ingameUIManager.ShowDamageText(new(c), symphonyPresenter.transform.position);
            enemyPresenter.Init(charaInitData.EnemyRepository, symphonyPresenter, new(_combatPipelineAsset.CombatHandler));
            enemyPresenter.OnCreatedEnemyModel += HandleEnemyCreated;
            ingameUIManager.RegisterComboCountHandler(new(comboEntity));

            // ゲーム開始の処理を行う。
            TestCardSpawn(cardInitData.CardUseCase);
            await inGameLoopUseCase.StartGameAsync();
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
                CardDataAsset cardDataAsset = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                CardData cardData = cardDataAsset.CreateCardData(_combatPipelineAsset);
                CardEntity instance = cardUseCase.CreateCard(cardData);

                instance.OnComplete += RandomDraw;
            }
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
