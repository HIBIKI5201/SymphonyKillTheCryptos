using SymphonyFrameWork;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    /// <summary>
    ///     カードを管理するクラス
    /// </summary>
    public class CardPresenter : MonoBehaviour, IInitializeAsync
    {
        public DeckManager DeckManager => _deckManager;

        Task IInitializeAsync.InitializeTask { get; set; }

        /// <summary>
        ///     カードを追加する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CardInstance AddCard(CardData data)
        {
            if (data == null)
            {
                Debug.LogWarning("カードデータがnullです");
                return null;
            }

            //カードのデータを生成
            var instance = _cardDrawer.GetNewCard(data);

            if (instance == null) return null;

            _deckManager.AddCardToDeck(instance);

            return instance;
        }

        async Task IInitializeAsync.InitializeAsync()
        {
            _cardDrawer = new();
            _deckManager = new();
            await (_deckManager as IInitializeAsync).DoInitialize();
        }

        [SerializeField]
        private CardDrawer _cardDrawer;

        private DeckManager _deckManager;

        private void Awake()
        {
            TestCardSpawn();
        }

        #region テストコード
        [Header("テストコード")]
        [SerializeField, Obsolete]
        private CardData[] _cardDatas;
        [SerializeField, Min(1)]
        private int _cardAmount = 3;
        private void TestCardSpawn()
        {
            for (int i = 0; i < _cardAmount; i++)
                RandomDraw();

            void RandomDraw(CardInstance ins = default)
            {
                Debug.Log("draw");
                var cardData = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                var instance = AddCard(cardData);

                instance.OnComplete += RandomDraw;
            }
        }
        #endregion
    }
}