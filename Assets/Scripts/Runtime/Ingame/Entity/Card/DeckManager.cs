using Cryptos.Runtime.System;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Ingame.Entity
{
    /// <summary>
    ///     デッキを管理するクラス
    /// </summary>
    public class DeckManager : MonoBehaviour, IInitializeAsync
    {
        public event Action<CardInstance> OnAddCardInstance;
        public event Action<CardInstance> OnRemoveCardInstance;

        Task IInitializeAsync.InitializeTask { get; set; }

        /// <summary>
        ///     カードを追加する
        /// </summary>
        /// <param name="data"></param>
        public CardInstance AddCardToDeck(CardData data)
        {
            if (data == null)
            {
                Debug.LogWarning("カードデータがnullです");
                return null;
            }

            //カードのデータを生成
            var instance = _cardDrawer.GetNewCard(data);

            if (instance == null) return null;

            _deckCardList.Add(instance);

            //終了イベント
            instance.OnComplete += CompletedEvent;

            OnAddCardInstance?.Invoke(instance);
            return instance;
        }

        async Task IInitializeAsync.InitializeAsync()
        {
            _wordManager = new WordManager();
            _cardDrawer = new CardDrawer(_wordManager);

            _inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            _playerManager = await ServiceLocator.GetInstanceAsync<SymphonyManager>();

            _inputBuffer.OnAlphabetKeyPressed += OnInputAlphabet;

            TestCardSpawn();
        }


        private CardDrawer _cardDrawer;
        private InputBuffer _inputBuffer;
        private WordManager _wordManager;

        private SymphonyManager _playerManager;

        private readonly List<CardInstance> _deckCardList = new();

        /// <summary>
        ///     アルファベット入力を受けた時のイベント
        /// </summary>
        /// <param name="alphabet"></param>
        private void OnInputAlphabet(char alphabet)
        {
            for (int i = 0; i < _deckCardList.Count; i++)
                _deckCardList[i].OnInputChar(alphabet);
        }

        /// <summary>
        ///     カードの入力が完了した時のイベント
        /// </summary>
        /// <param name="instance"></param>
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
            _deckCardList.Remove(instance);
            OnRemoveCardInstance?.Invoke(instance);
        }

        /// <summary>
        ///     全てのコンテンツを実行する
        /// </summary>
        /// <param name="contents"></param>
        private void InvokeContents(ICardContent[] contents)
        {
            foreach (var content in contents)
            {
                if (content == null) continue;

                content.TriggerEnterContent(_playerManager.gameObject);
            }
        }

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
                var instance = AddCardToDeck(cardData);

                instance.OnComplete += RandomDraw;
            }
        }
    }
}