using Cryptos.Runtime.Entity; // Add this using statement
using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.Entity.System.SaveData;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.Ingame.Card;
using Cryptos.Runtime.InfraStructure.Ingame.DataAsset;
using Cryptos.Runtime.InfraStructure.Ingame.Utility;
using Cryptos.Runtime.Presenter.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.Character.Enemy;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UI.Ingame.Manager;
using Cryptos.Runtime.UI.System.Audio;
using Cryptos.Runtime.UseCase;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.System;
using SymphonyFrameWork.System;
using System;
using System.Collections.Generic;
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
        [SerializeField, Tooltip("デフォルトのデータ")]
        private DefaultInGamePlayerData _defaultPlayerDataAsset;
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
            await InitializeUtility.WaitInitialize(ingameUIManager);

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

            PlayerDeckSaveData playerDeckSaveData = SaveDataSystem<PlayerDeckSaveData>.Data;
            PlayerMasterSaveData masterData = SaveDataSystem< PlayerMasterSaveData>.Data;
            // TODO: OutgameStartSequienceからplayerDeckSaveDataをDIで受け取るように修正
            
            // 選択されたデッキ名をPlayerDeckSaveDataから取得
            DeckNameValueObject selectedDeckName = masterData.DeckName;
            CardAddressValueObject[] selectedDeckAddresses = playerDeckSaveData.GetDeck(selectedDeckName);

            CardDeckEntity deckEntity;
            if (selectedDeckAddresses != null && selectedDeckAddresses.Length > 0)
            {
                // 選択されたデッキのアドレスからCardDeckEntityを生成
                CardData[] cardDatas = await CardDeckLoader.LoadDeck(selectedDeckAddresses, _combatPipelineAsset);
                  deckEntity = new CardDeckEntity(cardDatas);
            }
            else
            {
                // 選択されたデッキがない、または空の場合、デフォルトのデッキをロード
                Debug.LogWarning($"選択されたデッキ '{selectedDeckName}' が見つからないか空です。デフォルトデッキをロードします。");
                deckEntity = await _defaultPlayerDataAsset.CardDeckAsset.GetCardDeck(_combatPipelineAsset);
            }
            CardDeckUseCase deckUseCase = new(deckEntity, cardInitData.CardUseCase);

            CardExecutionUseCase cardExecutionUseCase = new(
                cardInitData.CardUseCase,
                symphonyPresenter
            );
            disposables.Add(cardExecutionUseCase);

            LevelUpPresenter levelUpPresenter = new(levelUseCase, ingameUIManager);

            IngameLoopPresenter ingameLoopPresenter = new(ingameUIManager);

            InGameLoopUseCase inGameLoopUseCase = new(
                cardInitData.CardUseCase,
                levelUseCase,
                waveUseCase,
                levelUpPresenter.HandleLevelUpSelectAsync,
                symphonyPresenter,
                waveSystemPresenter,
                inputPresenter,
                ingameLoopPresenter,
                disposables.ToArray()
            );

            ingameLoopPresenter.OnResultWindowReturnButtonClicked += GoToOutGameSceneInternal;

            CardPresenter cardPresenter = new(cardInitData.CardUseCase, cardExecutionUseCase, ingameUIManager);
            symphonyPresenter.Init(charaInitData.Symphony, symphonyData, cardExecutionUseCase, comboEntity);
            ingameUIManager.CreateHealthBar(new(charaInitData.Symphony, symphonyPresenter.transform, symphonyPresenter.destroyCancellationToken));
            charaInitData.Symphony.OnTakedDamage += c => ingameUIManager.ShowDamageText(new(c), symphonyPresenter.transform.position);
            enemyPresenter.Init(charaInitData.EnemyRepository, symphonyPresenter, new(_combatPipelineAsset.CombatHandler));
            enemyPresenter.OnCreatedEnemyModel += HandleEnemyCreated;
            ingameUIManager.RegisterComboCountHandler(new(comboEntity));

            // ゲーム開始の処理を行う。
            TestCardSpawn(deckUseCase);
            await inGameLoopUseCase.StartGameAsync();
        }

        private void TestCardSpawn(CardDeckUseCase cardUseCase)
        {
            for (int i = 0; i < _cardAmount; i++)
            {
                cardUseCase.DrawCard();
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



        private async void GoToOutGameSceneInternal()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;

            _gameUIManager.CloseResultWindow();

            await SceneLoader.UnloadScene(SceneListEnum.Stage.ToString());
            await SceneLoader.UnloadScene(SceneListEnum.Ingame.ToString());

            await SceneLoader.LoadScene(SceneListEnum.Outgame.ToString());
            await SceneLoader.LoadScene(SceneListEnum.Stage.ToString());
            SceneLoader.SetActiveScene(SceneListEnum.Stage.ToString());
        }

        private bool _isTransitioning;
    }
}
