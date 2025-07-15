using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.UseCase.Ingame.Card;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.Card
{
    /// <summary>
    ///     デッキを管理するクラス
    /// </summary>
    public class DeckManager : MonoBehaviour, IInitializeAsync
    {
        public event Action<CardEntity> OnAddCardInstance;
        public event Action<CardEntity> OnRemoveCardInstance;

        Task IInitializeAsync.InitializeTask { get; set; }

        /// <summary>
        ///     カードを追加する
        /// </summary>
        /// <param name="data"></param>
        public CardEntity AddCardToDeck(CardData data)
        {
            if (data == null)
            {
                Debug.LogWarning("カードデータがnullです");
                return null;
            }

            //カードのデータを生成
            CardEntity instance = _cardUseCase.CreateCard(data);

            if (instance == null) return null;

            _deckCardList.Add(instance);

            //終了イベント
            instance.OnComplete += CompletedEvent;

            OnAddCardInstance?.Invoke(instance);
            return instance;
        }

        async Task IInitializeAsync.InitializeAsync()
        {
            _inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            _playerManager = await ServiceLocator.GetInstanceAsync<SymphonyManager>();

            _cardUseCase = new(_wordDataBase);

            _inputBuffer.OnAlphabetKeyPressed += OnInputAlphabet;
        }

        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        private CardUseCase _cardUseCase;
        private readonly List<CardEntity> _deckCardList = new();

        private InputBuffer _inputBuffer;
        private SymphonyManager _playerManager;

        /// <summary>
        ///     アルファベット入力を受けた時のイベント
        /// </summary>
        /// <param name="alphabet"></param>
        private void OnInputAlphabet(char alphabet)
        {
            for (int i = 0; i < _deckCardList.Count; i++)
                _cardUseCase.InputCharToCard(_deckCardList[i], alphabet);
        }

        /// <summary>
        ///     カードの入力が完了した時のイベント
        /// </summary>
        /// <param name="instance"></param>
        private void CompletedEvent(CardEntity instance)
        {
            _cardUseCase.ExecuteCardEffect(instance, _playerManager.gameObject);
            RemoveCardFromDeck(instance);
        }

        /// <summary>
        ///     カードをデッキから削除する
        /// </summary>
        /// <param name="instance"></param>
        private void RemoveCardFromDeck(CardEntity instance)
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
            if (contents == null || contents.Length == 0) return;

            foreach (var content in contents)
            {
                if (content == null) continue;

                try
                {
                    content.TriggerEnterContent(_playerManager.gameObject);
                }
                catch (Exception e)
                {
                    Debug.LogError($"コンテンツの実行に失敗しました: {content.GetType().Name}\n{e.Message}");
                }
            }
        }
    }
}