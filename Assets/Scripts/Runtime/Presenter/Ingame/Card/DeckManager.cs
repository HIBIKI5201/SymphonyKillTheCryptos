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
        public event Action<WordEntity> OnAddCardInstance;
        public event Action<WordEntity> OnRemoveCardInstance;

        Task IInitializeAsync.InitializeTask { get; set; }

        /// <summary>
        ///     カードを追加する
        /// </summary>
        /// <param name="data"></param>
        public WordEntity AddCardToDeck(CardData data)
        {
            if (data == null)
            {
                Debug.LogWarning("カードデータがnullです");
                return null;
            }

            //カードのデータを生成
            WordEntity instance = _cardUseCase.CreateCard(data);

            if (instance == null) return null;

            _deckCardList.Add(instance);

            OnAddCardInstance?.Invoke(instance);
            return instance;
        }

        async Task IInitializeAsync.InitializeAsync()
        {
            if (_wordDataBase == null)
            {
                Debug.LogError("ワードのデータベースが設定されていません。");
                return;
            }

            _cardUseCase = new(_wordDataBase);
            _cardUseCase.OnCardCompleted += HandleCardCompleted;
            _cardUseCase.GetPlayer += () => _playerManager;

            _inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            _playerManager = await ServiceLocator.GetInstanceAsync<SymphonyManager>();

            if (_inputBuffer != null)
            {
                _inputBuffer.OnAlphabetKeyPressed += HandleInputAlphabet;
            }
        }

        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        private CardUseCase _cardUseCase;
        private readonly List<WordEntity> _deckCardList = new();

        private InputBuffer _inputBuffer;
        private SymphonyManager _playerManager;

        /// <summary>
        ///     アルファベット入力を受けた時のイベント
        /// </summary>
        /// <param name="alphabet"></param>
        private void HandleInputAlphabet(char alphabet)
        {
            for (int i = 0; i < _deckCardList.Count; i++)
                _cardUseCase.InputCharToCard(_deckCardList[i], alphabet);
        }

        /// <summary>
        ///     カードの入力が完了した時のイベント
        /// </summary>
        /// <param name="instance"></param>
        private void HandleCardCompleted(WordEntity instance)
        {
            RemoveCardFromDeck(instance);
        }

        /// <summary>
        ///     カードをデッキから削除する
        /// </summary>
        /// <param name="instance"></param>
        private void RemoveCardFromDeck(WordEntity instance)
        {
            _deckCardList.Remove(instance);
            OnRemoveCardInstance?.Invoke(instance);
        }
    }
}