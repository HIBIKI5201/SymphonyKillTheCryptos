using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Word;
using System;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    /// <summary>
    /// カードに関するアプリケーション固有のユースケースを処理するクラス。
    /// カードの生成、文字入力の処理、カード効果の実行などを担当します。
    /// </summary>
    public class CardUseCase
    {
        /// <summary>
        /// CardUseCaseの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="wordDataBase">ワードデータが格納されたデータベース</param>
        public CardUseCase(WordDataBase wordDataBase, CardDeckEntity deck)
        {
            _cardDrawer = new(wordDataBase);
            _cardDeckEntity = deck;
        }

        /// <summary>
        /// プレイヤーのIAttackableインターフェースを取得するためのイベント。
        /// </summary>
        public event Func<IAttackable> GetPlayer;
        public event Func<IHitable[]> GetTargets;

        /// <summary>
        /// カードの全てのワード入力が完了したときに発生するイベント。
        /// </summary>
        public event Action<CardEntity> OnCardCompleted;

        public event Action<CardEntity> OnCardAddedToDeck
        {
            add => _cardDeckEntity.OnAddCardInstance += value;
            remove => _cardDeckEntity.OnAddCardInstance -= value;
        }
        public event Action<CardEntity> OnCardRemovedFromDeck
        {
            add => _cardDeckEntity.OnRemoveCardInstance += value;
            remove => _cardDeckEntity.OnRemoveCardInstance -= value;
        }

        /// <summary>
        /// 新しいCardEntityインスタンスを生成します。
        /// </summary>
        /// <param name="data">生成するカードのデータ</param>
        /// <returns>生成されたCardEntityインスタンス。</returns>
        public CardEntity CreateCard(CardData data)
        {
            CardEntity entity = _cardDrawer.CreateNewCard(data);
            _cardDeckEntity.AddCardToDeck(entity);
            return entity;
        }

        public void InputCharToDeck(char input)
        {
            for (int i = 0; i < _cardDeckEntity.DeckCardList.Count; i++)
            {
                InputCharToCard(_cardDeckEntity.DeckCardList[i], input);
            }
        }

        /// <summary>
        /// 指定されたCardEntityに対して文字入力を処理します。
        /// カードのワード入力が完了した場合、カード効果を実行し、OnCardCompletedイベントを発火します。
        /// </summary>
        /// <param name="cardEntity">文字入力を処理するCardEntity</param>
        /// <param name="input">入力された文字</param>
        public void InputCharToCard(CardEntity cardEntity, char input)
        {
            //入力をエンティティに渡す
            if (cardEntity.WordEntity.OnInputChar(input)) // WordEntityのOnInputCharを呼び出すように変更
            {
                //入力が完了した時の処理
                ExecuteCardEffect(cardEntity,
                    player: GetPlayer?.Invoke(),
                    targets: GetTargets?.Invoke());

                _cardDeckEntity.RemoveCardFromDeck(cardEntity);
                OnCardCompleted?.Invoke(cardEntity);
            }
        }

        /// <summary>
        /// 指定されたCardEntityのカード効果を実行します。
        /// </summary>
        /// <param name="cardEntity">効果を実行するCardEntity</param>
        /// <param name="player">効果の対象となるプレイヤー（IAttackable）</param>
        /// <param name="targets">効果の対象となるターゲット（IHitableの可変長引数）</param>
        public void ExecuteCardEffect(CardEntity cardEntity, IAttackable player, params IHitable[] targets)
        {
            if (player == null)
            {
                Debug.LogError("Player is null");
                return;
            }

            ICardContent[] contents = cardEntity.Data.Contents; // CardDataへのアクセスを修正

            if (contents == null || contents.Length == 0) return;

            foreach (var content in contents)
            {
                if (content == null) continue;

                try
                {
                    content.Execute(player, targets);
                }
                catch (Exception e)
                {
                    Debug.LogError($"コンテンツの実行に失敗しました: {content.GetType().Name}\n{e.Message}\nstack trace\n{e.StackTrace}");
                }
            }
        }

        private readonly CardDeckEntity _cardDeckEntity;
        private readonly CardDrawer _cardDrawer;
    }
}