using Cryptos.Runtime.Entity.Ingame.Character;
using System.Collections.Generic;
using System.Linq;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    /// <summary>
    /// 戦闘における一連のダメージ計算処理を統括するクラスです。
    /// 複数のICombatHandlerを登録し、順番に実行することで最終的なダメージを算出します。
    /// </summary>
    public class CombatProcessor
    {
        /// <summary>
        /// 登録された全ての戦闘ハンドラーを実行し、最終的な戦闘結果を計算します。
        /// </summary>
        /// <param name="attacker">攻撃側のキャラクター。</param>
        /// <param name="target">防御側のキャラクター。</param>
        /// <returns>全ての計算が完了した後の最終的なCombatContext。</returns>
        public static CombatContext Execute(IAttackable attacker, IHittable target, params ICombatHandler[] handlers)
        {
            CombatContext context = new CombatContext(attacker.AttackableData, target.HittableData, attacker.AttackableData.AttackPower);

            foreach (var handler in handlers)
            {
                context = handler.Execute(context);
            }

            return context;
        }
    }
}
