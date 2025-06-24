using Cryptos.Runtime.System;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    public class DeckManager : MonoBehaviour, IInitializeAsync
    {
        public event Action<CardInstance> OnAddCardInstance;
        public event Action<CardInstance> OnRemoveCardInstance;

        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDatabase;

        Task IInitializeAsync.InitializeTask { get; set; }

        private InputBuffer _inputBuffer;
        private WordManager _wordManager;

        private readonly List<CardInstance> _deckCard = new();

        [SerializeField, Obsolete]
        private CardData[] _cardDatas;

        async Task IInitializeAsync.InitializeAsync()
        {
            _wordManager = new WordManager();
            _inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();

            RandomDraw();
            RandomDraw();
            RandomDraw();

            void RandomDraw()
            {
                Debug.Log("draw");
                var cardData = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                var instance = AddCardToDeck(cardData);

                instance.OnComplete += RandomDraw;
            }
        }

        /// <summary>
        ///     カードを追加する
        /// </summary>
        /// <param name="data"></param>
        public CardInstance AddCardToDeck(CardData data)
        {
            //カードの難易度までのワードを取得
            WordData[] words = _wordDatabase.WordData.Take(data.CardDifficulty).ToArray();

            //カードを生成
            CardInstance instance = new(data, words, _wordManager);
            _inputBuffer.OnAlphabetKeyPressed += instance.OnInputChar;
            _deckCard.Add(instance);

            //終わったら削除する
            instance.OnComplete += () => RemoveCardFromDeck(instance);

            OnAddCardInstance?.Invoke(instance);
            return instance;
        }

        /// <summary>
        ///     カードをデッキから削除する
        /// </summary>
        /// <param name="instance"></param>
        public void RemoveCardFromDeck(CardInstance instance)
        {
            _inputBuffer.OnAlphabetKeyPressed -= instance.OnInputChar;
            _deckCard.Remove(instance);
            OnRemoveCardInstance?.Invoke(instance);
        }
    }
}