using Cryptos.Runtime.Entity.Ingame.Character;
using System.Collections.Generic;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    /// <summary>
    /// 戦闘における一連のダメージ計算処理を統括するクラスです。
    /// 複数のICombatHandlerを登録し、順番に実行することで最終的なダメージを算出します。
    /// </summary>
    internal class CombatProcessor
    {
        /// <summary>
        /// 計算処理を行うハンドラーをリストに追加します。
        /// </summary>
        /// <param name="handler">追加する戦闘ハンドラー。</param>
        /// <returns>メソッドチェーンのために自身のインスタンスを返します。</returns>
        public CombatProcessor AddTo(ICombatHandler handler)
        {
            _combatHandlers.Add(handler);
            return this;
        }

        /// <summary>
        /// 登録された全ての戦闘ハンドラーを実行し、最終的な戦闘結果を計算します。
        /// </summary>
        /// <param name="attacker">攻撃側のキャラクター。</param>
        /// <param name="target">防御側のキャラクター。</param>
        /// <returns>全ての計算が完了した後の最終的なCombatContext。</returns>
        public CombatContext Execute(IAttackable attacker, IHittable target)
        {
            CombatContext context = new CombatContext(attacker.AttackableData, target.HittableData, attacker.AttackableData.AttackPower);

            foreach(var handler in _combatHandlers)
            {
                context = handler.Execute(context);
            }

            return context;
        }

        private readonly List<ICombatHandler> _combatHandlers = new();
    }
}
