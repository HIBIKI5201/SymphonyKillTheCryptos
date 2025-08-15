using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    /// <summary>
    /// カードに関するアプリケーション固有のユースケースを処理するクラスです。
    /// カードの生成、文字入力の処理、カード効果の実行などを担当します。
    /// </summary>
    public class CardUseCase
    {
        /// <summary>
        /// CardUseCaseの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="wordDataBase">ワードデータが格納されたデータベース。</param>
        /// <param name="deck">カードデッキのエンティティ。</param>
        public CardUseCase(WordDataBase wordDataBase, CardDeckEntity deck)
        {
            _cardDeckEntity = deck;
            _cardDrawer = new(wordDataBase, _wordManager);
        }

        /// <summary> プレイヤーのICharacterインターフェースを取得するためのイベントです。 </summary>
        public event Func<ICharacter> GetPlayer;
        /// <summary> 攻撃対象のICharacterインターフェース配列を取得するためのイベントです。 </summary>
        public event Func<ICharacter[]> GetTargets;
        /// <summary> カードの全てのワード入力が完了したときに発生するイベントです。 </summary>
        public event Action<CardEntity> OnCardCompleted;
        /// <summary> カードがデッキに追加されたときに発生するイベントです。 </summary>
        public event Action<CardEntity> OnCardAddedToDeck { add => _cardDeckEntity.OnAddCardInstance += value; remove => _cardDeckEntity.OnAddCardInstance -= value; }
        /// <summary> カードがデッキから削除されたときに発生するイベントです。 </summary>
        public event Action<CardEntity> OnCardRemovedFromDeck { add => _cardDeckEntity.OnRemoveCardInstance += value; remove => _cardDeckEntity.OnRemoveCardInstance -= value; }

        /// <summary>
        /// 新しいCardEntityインスタンスを生成し、デッキに追加します。
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

            _cardDeckEntity.AddCardToDeck(entity);
            return entity;
        }

        /// <summary>
        /// デッキ内の全てのカードに文字入力を試みます。
        /// </summary>
        /// <param name="input">入力された文字。</param>
        public void InputCharToDeck(char input)
        {
            foreach (var card in _cardDeckEntity.DeckCardList.ToArray())
            {
                card.WordEntity.OnInputChar(input);
            }
        }

        /// <summary>
        /// 指定されたカード効果を実行します。
        /// </summary>
        /// <param name="contents">実行するICardContentの配列。</param>
        public void ExecuteCardEffect(ICardContent[] contents)
        {
            ICharacter[] players = new ICharacter[1] { GetPlayer?.Invoke() };
            ICharacter[] targets = GetTargets?.Invoke() ?? Array.Empty<ICharacter>();

            if (players == null || players.Length <= 0 || players[0] == null)
            {
                Debug.LogError("Player is null.");
                return;
            }

            if (contents == null || contents.Length <= 0)
            {
                Debug.LogWarning("No contents to execute.");
                return;
            }

            foreach (var content in contents)
            {
                if (content == null) continue;

                try
                {
                    content.Execute(players, targets);
                }
                catch (Exception e)
                {
                    Debug.LogError($"コンテンツの実行に失敗しました: {content.GetType().Name}{e.Message}stack trace{e.StackTrace}");
                }
            }
        }

        private readonly CardDeckEntity _cardDeckEntity;
        private readonly CardDrawer _cardDrawer;
        private readonly WordManager _wordManager = new();
        private readonly Dictionary<CardEntity, WordData[]> _cardWordCandidates = new();

        /// <summary>
        /// 現在の単語入力が完了した際の処理です。
        /// 次の単語をセットし、ワードマネージャーを更新します。
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
        /// カードの全てのワード入力が完了した際の処理です。
        /// カードをデッキから削除し、関連イベントを発行します。
        /// </summary>
        /// <param name="cardEntity">完了したカードエンティティ。</param>
        private void HandleCardCompleted(CardEntity cardEntity)
        {
            // カードがデッキから削除され、ワードが管理から解放されます。
            _wordManager.RemoveWord(cardEntity.WordEntity.CurrentWord);
            _cardDeckEntity.RemoveCardFromDeck(cardEntity);
            _cardWordCandidates.Remove(cardEntity);

            // 残りのカードのワードエンティティのインデックスをリセットします。
            foreach (CardEntity card in _cardDeckEntity.DeckCardList)
            {
                card.WordEntity.ResetIndex();
            }

            OnCardCompleted?.Invoke(cardEntity);
        }
    }
}
