using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    /// <summary>
    /// クリティカルヒットの計算を行う戦闘ハンドラーです。
    /// 100%を超えるクリティカル確率にも対応します。
    /// </summary>
    public class CriticalCalcHandler : ICombatHandler
    {
        /// <summary>
        /// CombatContextを受け取り、クリティカルヒット計算を適用した新しいContextを返します。
        /// </summary>
        /// <param name="context">現在の戦闘コンテキスト。</param>
        /// <returns>クリティカル計算後の新しい戦闘コンテキスト。</returns>
        public CombatContext Execute(CombatContext context)
        {
            // 100%を超えるクリティカル確率を考慮し、確定で発生するクリティカル回数を計算します。
            int criticalWeight = Mathf.FloorToInt(context.AttackerData.CriticalChance / 100);

            // 100%未満の端数のクリティカル確率を計算します。
            float remainCriticalChance = context.AttackerData.CriticalChance % 100;

            // 端数確率でクリティカルが発生したか判定します。
            if (Random.Range(0, 100) < remainCriticalChance)
            {
                criticalWeight++;
            }

            // 最終的なダメージ倍率を計算します。
            // CriticalDamageは倍率（例: 2.0で2倍ダメージ）なので、増加分を計算するために-1します。
            float damageScale = 1 + (criticalWeight * (context.AttackerData.CriticalDamage - 1));
            float damage = context.Damage * damageScale;

            return new CombatContext(context.AttackerData, context.TargetData, damage, criticalWeight);
        }
    }
}
