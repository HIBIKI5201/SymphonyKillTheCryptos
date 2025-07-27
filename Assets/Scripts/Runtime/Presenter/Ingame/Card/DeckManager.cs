using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Presenter.Character.Player;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Card;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System;
using System.Linq;
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

            _inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            _playerManager = await ServiceLocator.GetInstanceAsync<SymphonyManager>();
            _enemyManager = await ServiceLocator.GetInstanceAsync<EnemyManager>();

            if (_inputBuffer != null)
            {
                _inputBuffer.OnAlphabetKeyPressed += HandleInputAlphabet;
            }

            if (_playerManager == null)
            {
                Debug.LogError("プレイヤーマネージャーが取得できませんでした。");
                return;
            }
            if (_enemyManager == null)
            {
                Debug.LogError("エネミーマネージャーが取得できませんでした。");
                return;
            }

            _cardUseCase = new(_wordDataBase, _deckEntity);
            _cardUseCase.GetPlayer += () => _playerManager.Entity;
            _cardUseCase.GetTargets += () => _enemyManager.AllEnemies.ToArray();
        }

        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        private CardUseCase _cardUseCase;

        private CardDeckEntity _deckEntity = new();
        private InputBuffer _inputBuffer;
        private SymphonyManager _playerManager;
        private EnemyManager _enemyManager;

        /// <summary>
        ///     アルファベット入力を受けた時のイベント
        /// </summary>
        /// <param name="alphabet"></param>
        private void HandleInputAlphabet(char alphabet)
        {
            for (int i = 0; i < _deckEntity.DeckCardList.Count; i++)
                _cardUseCase.InputCharToCard(_deckEntity.DeckCardList[i], alphabet);
        }
    }
}