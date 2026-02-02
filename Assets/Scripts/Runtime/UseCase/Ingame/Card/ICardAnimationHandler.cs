using System;

namespace Cryptos.Runtime.UseCase.Ingame.Card
{
    /// <summary>
    /// カードアニメーションの制御とイベント通知のためのインターフェースである。
    /// </summary>
    public interface ICardAnimationHandler
    {
        /// <summary>
        /// 指定されたアニメーションクリップIDでスキルアニメーションをアクティブにする。
        /// </summary>
        /// <param name="animationClipID">アクティブにするアニメーションクリップのID。</param>
        void ActiveSkill(int animationClipID);

        /// <summary>
        /// スキルトリガーイベント。アニメーション中の特定のタイミングで発生する。
        /// </summary>
        event Action<int> OnSkillTriggered;

        /// <summary>
        /// スキルアニメーション終了イベント。
        /// </summary>
        event Action OnSkillEnded;
    }
}
