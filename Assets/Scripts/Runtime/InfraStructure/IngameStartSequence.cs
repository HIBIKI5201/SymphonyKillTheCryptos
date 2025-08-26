using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Ingame.System;
using Cryptos.Runtime.Presenter.Character.Enemy;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UI.Ingame;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.System;
using SymphonyFrameWork.System;
using System;
using System.Linq;
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
        private SymphonyData _symphonyData;
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        [Header("テストコード")]
        [SerializeField]
        private CardData[] _cardDatas;
        [SerializeField, Min(1)]
        private int _cardAmount = 3;

        [Space]
        [SerializeField]
        private WaveEntity[] _waveEntities;

        private CardUseCase _cardUseCase;
        private CardPresenter _cardPresenter;
        private CharacterEntity<SymphonyData> _symphony;
        private EnemyRepository _enemyRepo;
        private WaveSystemPresenter _waveSystem;

        public async void GameInitialize()
        {
            
            CharacterEntity<SymphonyData> symphony = new(_symphonyData);

            EnemyRepository enemyRepo = new();

            WaveSystemPresenter waveSystem = new(_waveEntities, enemyRepo);

            CardUseCase cardUseCase = new(_wordDataBase, new());
            cardUseCase.GetPlayer += () => symphony;
            cardUseCase.GetTargets += () => enemyRepo.AllEnemies.ToArray();

            InputBuffer inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            inputBuffer.OnAlphabetKeyPressed += cardUseCase.InputCharToDeck;

            PlayerPathContainer playerPathContainer = await ServiceLocator.GetInstanceAsync<PlayerPathContainer>();

            IngameUIManager ingameUIManager = await ServiceLocator.GetInstanceAsync<IngameUIManager>();
            await InitializeUtility.WaitInitialize(ingameUIManager);
            CardPresenter cardPresenter = new CardPresenter(cardUseCase, ingameUIManager);

            SymphonyPresenter symphonyPresenter = await ServiceLocator.GetInstanceAsync<SymphonyPresenter>();
            symphonyPresenter.Init(cardUseCase, playerPathContainer);

            EnemyPresenter enemyPresenter = await ServiceLocator.GetInstanceAsync<EnemyPresenter>();
            enemyPresenter.Init(enemyRepo, symphonyPresenter);

            _waveSystem = waveSystem;
            _symphony = symphony;
            _enemyRepo = enemyRepo;
            _cardUseCase = cardUseCase;
            _cardPresenter = cardPresenter;

            TestCardSpawn();

            SymphonyFrameWork.Debugger.SymphonyDebugHUD.AddText($"screen time{Time.time}");
        }

        private void TestCardSpawn()
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
                var instance = _cardUseCase.CreateCard(cardData);

                instance.OnComplete += RandomDraw;
            }
        }
    }
}
