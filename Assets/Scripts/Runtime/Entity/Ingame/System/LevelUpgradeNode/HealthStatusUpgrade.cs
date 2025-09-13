using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    /// <summary>
    /// 体力をアップグレードするレベルアップ効果。
    /// </summary>
    public class HealthStatusUpgrade : LevelUpgradeStatusEffect
    {
        /// <summary>
        /// ステータス効果を適用します。
        /// </summary>
        /// <param name="target">効果を適用する対象。</param>
        public override void ApplyStatusEffect(TentativeCharacterData target)
        {
            target.SetNewBuff(TentativeCharacterData.BuffType.MaxHealth,
                _healthIncreaseAmount);
        }

        [SerializeField, Min(0), Tooltip("体力増加量。(%)")]
        private float _healthIncreaseAmount = 0;
    }
}
