using Cryptos.Runtime.System;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using System;
using System.Collections.Generic;
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
        }

        private async void Start()
        {
            await SymphonyTask.WaitUntil(() => (this as IInitializeAsync).IsDone);

            foreach (var item in _cardDatas) //開発用機能
                AddCardToDeck(item);
        }

        /// <summary>
        ///     カードを追加する
        /// </summary>
        /// <param name="data"></param>
        public void AddCardToDeck(CardData data)
        {
            List<WordData> words = new();

            //カードの難易度までのワードを追加
            for (int i = 0; i < data.CardDifficulty; i++)
            {
                words.AddRange(_wordDatabase[i]);
            }

            if (words.Count <= 0)
            {
                Debug.LogError("ワードが一つもありません");
                return;
            }

            //カードを生成
            CardInstance instance = new(data, words.ToArray(), _wordManager);
            _inputBuffer.OnAlphabetKeyPressed += instance.OnInputChar;
            _deckCard.Add(instance);

            //終わったら削除する
            instance.OnCompleteInput += () => RemoveCardFromDeck(instance);

            OnAddCardInstance?.Invoke(instance);
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