using Cryptos.Runtime.Entity.Ingame.Card;
using System;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    /// <summary>
    /// カードの実行キューとアニメーション連携を管理するユースケースである。
    /// </summary>
    public class CardExecutionUseCase : IDisposable
    {
        private readonly CardUseCase _cardUseCase;
        private readonly CardExecutionQueueEntity _cardQueue;
        private readonly ICardAnimationHandler _cardAnimationHandler;

        public CardExecutionUseCase(
            CardUseCase cardUseCase,
            ICardAnimationHandler cardAnimationHandler)
        {
            _cardUseCase = cardUseCase;
            _cardAnimationHandler = cardAnimationHandler;

            _cardQueue = new();

            _cardUseCase.OnCardCompleted += HandleCardCompleted;
            _cardAnimationHandler.OnSkillTriggered += HandleSkillTriggered;
            _cardAnimationHandler.OnSkillEnded += HandleSkillEnded;
        }

        /// <summary>
        /// 使用キューをリセットする。
        /// </summary>
        public void Reset()
        {
            _cardQueue.Clear();
        }

        /// <summary>
        ///     イベント購読を解除する。
        /// </summary>
        public void Dispose()
        {
            _cardUseCase.OnCardCompleted -= HandleCardCompleted;
            _cardAnimationHandler.OnSkillTriggered -= HandleSkillTriggered;
            _cardAnimationHandler.OnSkillEnded -= HandleSkillEnded;
        }

        /// <summary>
        ///     カードの入力が完了した際の処理。
        /// </summary>
        private void HandleCardCompleted(CardEntity cardEntity)
        {
            if (cardEntity == null) { return; }

            _cardQueue.Enqueue(cardEntity);

            // もしスキル中でなければ発動する。
            if (_cardQueue.Count <= 1)
            {
                _cardAnimationHandler.ActiveSkill(cardEntity.AnimationClipID);
            }
        }

        /// <summary>
        ///     スキルアニメーションのトリガーのタイミングで効果を発動する。
        /// </summary>
        private void HandleSkillTriggered(int index)
        {
            if (!_cardQueue.TryPeek(out var card)) { return; }

            _cardUseCase.ExecuteCardEffect(card.GetContents(index));
        }

        /// <summary>
        ///     スキルアニメーション終了を処理する。
        /// </summary>
        private void HandleSkillEnded()
        {
            if (!_cardQueue.TryDequeue(out _)) { return; }

            // もし残っていれば発動する。
            if (_cardQueue.TryPeek(out var nextCard))
            {
                _cardAnimationHandler.ActiveSkill(nextCard.AnimationClipID);
            }
        }
    }
}
