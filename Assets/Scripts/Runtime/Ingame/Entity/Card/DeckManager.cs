using Cryptos.Runtime.System;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
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

        private SymphonyManager _playerManager;

        private readonly List<CardInstance> _deckCard = new();

        [SerializeField, Obsolete]
        private CardData[] _cardDatas;

        async Task IInitializeAsync.InitializeAsync()
        {
            _wordManager = new WordManager();
            _inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            _playerManager = await ServiceLocator.GetInstanceAsync<SymphonyManager>();

            RandomDraw();
            RandomDraw();
            RandomDraw();

            void RandomDraw(CardInstance ins = default)
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
            if (Mathf.Abs(data.WordRange.x - data.WordRange.y) < 1)
            {
                Debug.LogWarning($"{data.name}のWordRnageが不適切です\n二つの距離は");
                return null;
            }

            //カードの難易度までのワードを取得
            WordData[] words = _wordDatabase
                .WordData[data.WordRange.x..data.WordRange.y].ToArray();

            //カードを生成
            CardInstance instance = new(data, words, _wordManager);
            _inputBuffer.OnAlphabetKeyPressed += instance.OnInputChar;
            _deckCard.Add(instance);

            //終了イベント
            instance.OnComplete += CompletedEvent;

            OnAddCardInstance?.Invoke(instance);
            return instance;
        }

        private void CompletedEvent(CardInstance instance)
        {
            InvokeContents(instance.CardData.Contents);
            RemoveCardFromDeck(instance);
        }


        /// <summary>
        ///     カードをデッキから削除する
        /// </summary>
        /// <param name="instance"></param>
        private void RemoveCardFromDeck(CardInstance instance)
        {
            _inputBuffer.OnAlphabetKeyPressed -= instance.OnInputChar;
            _deckCard.Remove(instance);
            OnRemoveCardInstance?.Invoke(instance);
        }

        /// <summary>
        ///     全てのコンテンツを実行する
        /// </summary>
        /// <param name="contents"></param>
        private void InvokeContents(ICardContent[] contents)
        {
            foreach (var card in contents)
            {
                card.TriggerEnterContent(_playerManager.gameObject);
            }
        }
    }
}