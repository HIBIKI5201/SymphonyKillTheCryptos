using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.CombatSystem
{
    /// <summary>
    ///     ターゲットの防御力によるダメージ減衰を適用する。
    /// </summary>
    public class ArmorCalcHandler : ICombatHandler
    {
        public CombatContext Execute(CombatContext context)
        {
            float armor = context.TargetData.Armor;

            // armor = _armorThreshold のとき軽減率50%になる式。
            float reductionRate = armor / (_armorThreshold + armor);

            // 最終的に与えるダメージ。
            float finalDamage = context.Damage * (1f - reductionRate);

            return new CombatContext(context, finalDamage);
        }

        [SerializeField, Tooltip("装甲値がこの値と等しい場合、減衰率が50%になる基準点")]
        private float _armorThreshold = 100;
    }
}
