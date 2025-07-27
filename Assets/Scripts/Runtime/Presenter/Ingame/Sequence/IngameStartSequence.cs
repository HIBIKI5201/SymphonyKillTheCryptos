using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Character;
using JetBrains.Annotations;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Sequence
{
    [Serializable]
    public class IngameStartSequence
    {
        [Header("テストコード")]
        [SerializeField]
        private CardData[] _cardDatas;
        [SerializeField, Min(1)]
        private int _cardAmount = 3;

        [Space]
        [SerializeField]
        private EnemyData _enemyData;
        private int _enemyAmount = 3;

        public async Task StartSequence()
        {
            DeckManager deck = await ServiceLocator.GetInstanceAsync<DeckManager>();
            SymphonyManager player = await ServiceLocator.GetInstanceAsync<SymphonyManager>();
            EnemyManager enemy = await ServiceLocator.GetInstanceAsync<EnemyManager>();

            if (deck is IInitializeAsync initialize)
            {
                await SymphonyTask.WaitUntil(() => initialize.IsDone);
            }

            TestCardSpawn(deck);

            TestEnemySpawn(enemy);
        }

        private void TestCardSpawn(DeckManager deckManager)
        {
            if (_cardDatas == null || _cardDatas.Length == 0)
            {
                Debug.LogWarning("カードデータが設定されていません。");
                return;
            }

            for (int i = 0; i < _cardAmount; i++)
                RandomDraw();

            void RandomDraw()
            {
                Debug.Log("draw");
                var cardData = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                var instance = deckManager.AddCardToDeck(cardData);

                instance.OnComplete += RandomDraw;
            }
        }

        private void TestEnemySpawn(EnemyManager enemyManager)
        {
            if (enemyManager == null)
            {
                Debug.LogWarning("EnemyManagerが設定されていません。");
                return;
            }

            for (int i = 0; i < _enemyAmount; i++)
            {
                var enemy = enemyManager.CreateEnemy(_enemyData);
                if (enemy == null)
                {
                    Debug.LogWarning("エネミーの生成に失敗しました。");
                    continue;
                }
            }
        }
    }
}
