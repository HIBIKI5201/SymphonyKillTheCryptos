using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    /// <summary>
    ///     カードに関するアプリケーション固有のユースケースを処理するクラスです。
    ///     カードの生成、文字入力の処理、カード効果の実行などを担当します。
    /// </summary>
    public class CardUseCase
    {
        /// <summary>
        ///     CardUseCaseの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="wordDataBase">ワードデータが格納されたデータベース。</param>
        /// <param name="deck">カードデッキのエンティティ。</param>
        public CardUseCase(WordDataBase wordDataBase, ComboEntity comboEntity)
        {
            _cardHandEntity = new();
            _comboEntity = comboEntity;
            _cardDrawer = new(wordDataBase, _wordManager);
        }

        /// <summary> プレイヤーのICharacterインターフェースを取得するためのイベントです。 </summary>
        public event Func<ICharacter> GetPlayer;
        /// <summary> 攻撃対象のICharacterインターフェース配列を取得するためのイベントです。 </summary>
        public event Func<ICharacter[]> GetTargets;
        /// <summary> カードの全てのワード入力が完了したときに発生するイベントです。 </summary>
        public event Action<CardEntity> OnCardCompleted;
        /// <summary> カードがデッキに追加されたときに発生するイベントです。 </summary>
        public event Action<CardEntity> OnCardAddedToDeck { add => _cardHandEntity.OnAddCardInstance += value; remove => _cardHandEntity.OnAddCardInstance -= value; }
        /// <summary> カードがデッキから削除されたときに発生するイベントです。 </summary>
        public event Action<CardEntity> OnCardRemovedFromDeck { add => _cardHandEntity.OnRemoveCardInstance += value; remove => _cardHandEntity.OnRemoveCardInstance -= value; }

        /// <summary>
        ///     新しいCardEntityインスタンスを生成し、デッキに追加します。
        /// </summary>
        /// <param name="data">生成するカードのデータ。</param>
        /// <returns>生成されたCardEntityインスタンス。</returns>
        public CardEntity CreateCard(CardData data)
        {
            CardEntity entity = _cardDrawer.CreateNewCard(data, out var availableWords);
            if (entity == null) return null;

            _cardWordCandidates.Add(entity, availableWords);

            entity.WordEntity.OnCurrentWordCompleted += () => HandleCurrentWordCompleted(entity);
            entity.OnComplete += () => HandleCardCompleted(entity);

            _cardHandEntity.AddCardToDeck(entity);
            return entity;
        }

        /// <summary>
        ///     デッキ内の全てのカードに文字入力を試みます。
        /// </summary>
        /// <param name="input">入力された文字。</param>
        public void InputCharToDeck(char input)
        {
            for (int i = _cardHandEntity.CardList.Count - 1; i >= 0; i--)
            {
                // ループ中にリストの要素数が変わる可能性があるため、範囲チェックを行います。
                if (i < _cardHandEntity.CardList.Count)
                {
                    CardEntity card = _cardHandEntity.CardList[i];
                    card.WordEntity.OnInputChar(input);
                }
            }
        }

        /// <summary>
        ///     指定されたカード効果を実行します。
        /// </summary>
        /// <param name="contents">実行するICardContentの配列。</param>
        public void ExecuteCardEffect(CardData.CardContents contents)
        {
            ICharacter[] players = new ICharacter[1] { GetPlayer?.Invoke() };
            ICharacter[] targets = GetTargets?.Invoke() ?? Array.Empty<ICharacter>();

            if (players == null || players.Length <= 0 || players[0] == null)
            {
                Debug.LogError("Player is null.");
                return;
            }

            contents.ExcuteAllContent(players, targets);
        }

        private readonly CardHandEntity _cardHandEntity;
        private readonly CardDrawer _cardDrawer;
        private readonly ComboEntity _comboEntity;
        private readonly WordManager _wordManager = new();
        private readonly Dictionary<CardEntity, WordData[]> _cardWordCandidates = new();

        /// <summary>
        ///     現在の単語入力が完了した際の処理です。
        ///     次の単語をセットし、ワードマネージャーを更新します。
        /// </summary>
        /// <param name="cardEntity">処理対象のカードエンティティ。</param>
        private void HandleCurrentWordCompleted(CardEntity cardEntity)
        {
            _wordManager.RemoveWord(cardEntity.WordEntity.CurrentWord);

            if (!_cardWordCandidates.TryGetValue(cardEntity, out var candidates)) return;

            var nextWordData = _wordManager.GetAvailableWord(candidates);
            if (nextWordData.word == null)
            {
                Debug.LogError("次の単語が見つかりませんでした。");
                HandleCardCompleted(cardEntity);
                return;
            }

            cardEntity.WordEntity.SetNewWord(nextWordData.word, nextWordData.difficulty);
            _wordManager.AddWord(nextWordData.word);
        }

        /// <summary>
        ///     カードの全てのワード入力が完了した際の処理です。
        ///     カードをデッキから削除し、関連イベントを発行します。
        /// </summary>
        /// <param name="cardEntity">完了したカードエンティティ。</param>
        private void HandleCardCompleted(CardEntity cardEntity)
        {
            // カードがデッキから削除され、ワードが管理から解放されます。
            _wordManager.RemoveWord(cardEntity.WordEntity.CurrentWord);
            _cardHandEntity.RemoveCardFromDeck(cardEntity);
            _cardWordCandidates.Remove(cardEntity);
            _comboEntity.Increment();

            // 残りのカードのワードエンティティのインデックスをリセットします。
            foreach (CardEntity card in _cardHandEntity.CardList)
            {
                card.WordEntity.ResetIndex();
            }

            OnCardCompleted?.Invoke(cardEntity);
        }
    }
}
