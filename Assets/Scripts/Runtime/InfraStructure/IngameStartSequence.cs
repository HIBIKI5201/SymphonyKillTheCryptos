using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.Ingame.Utility;
using Cryptos.Runtime.Ingame.System;
using Cryptos.Runtime.Presenter.Character.Enemy;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UI.Ingame;
using Cryptos.Runtime.UseCase.Ingame.Card;
using Cryptos.Runtime.UseCase.Ingame.System;
using SymphonyFrameWork.System;
using System;
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
        private SymphonyData _symphonyData;
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        [SerializeField]
        private LevelSelectData _levelSelectData;

        [Header("テストコード")]
        [SerializeField]
        private CardData[] _cardDatas;
        [SerializeField, Min(1)]
        private int _cardAmount = 3;

        [Space]
        [SerializeField]
        private WaveEntity[] _waveEntities;

        public async void GameInitialize()
        {
            CharacterInitializer.CharacterInitializationData charaInitData =
                CharacterInitializer.Initialize(_symphonyData);

            CardInitializer.CardInitializationData cardInitData =
                CardInitializer.Initialize(_wordDataBase,
                charaInitData.Symphony, charaInitData.EnemyRepository);

            LevelUseCase levelUseCase =
                new LevelUseCase(_levelSelectData, LevelAsync);

            InputBuffer inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            inputBuffer.OnAlphabetKeyPressed += cardInitData.CardUseCase.InputCharToDeck;

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

            TestCardSpawn(cardInitData.CardUseCase);
            waveSystem.GameStart();

            SymphonyFrameWork.Debugger.SymphonyDebugHUD.AddText($"screen time{Time.time}");
        }

        private async Task<string> LevelAsync(string[] cards)
        {
            Debug.Log($"候補カード {string.Join(' ', cards)}");

            await Awaitable.WaitForSecondsAsync(2f);

            string selectedCard = cards[UnityEngine.Random.Range(0, cards.Length)];

            Debug.Log($"レベルアップカードを選択しました。{selectedCard}");

            return selectedCard;
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
