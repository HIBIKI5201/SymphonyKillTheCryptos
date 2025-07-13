using Cryptos.Runtime.Entity.Ingame.Card;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using UnityEngine;

namespace Cryptos.Runtime
{
    public class CardDeckDev : MonoBehaviour
    {
        [Header("テストコード")]
        [SerializeField]
        private CardData[] _cardDatas;
        [SerializeField, Min(1)]
        private int _cardAmount = 3;

        private async void Start()
        {
            DeckManager deckManager = await ServiceLocator.GetInstanceAsync<DeckManager>();
            await SymphonyTask.WaitUntil(() => (deckManager as IInitializeAsync).IsDone);

            TestCardSpawn(deckManager);
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

            void RandomDraw(CardEntity ins = default)
            {
                Debug.Log("draw");
                var cardData = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                var instance = deckManager.AddCardToDeck(cardData);

                instance.OnComplete += RandomDraw;
            }
        }
    }
}
