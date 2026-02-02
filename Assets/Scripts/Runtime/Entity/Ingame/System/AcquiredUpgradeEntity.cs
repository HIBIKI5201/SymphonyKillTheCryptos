using System.Collections.Generic;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    /// <summary>
    /// 取得済みのレベルアップグレードノードの状態を管理するエンティティ。
    /// </summary>
    public class AcquiredUpgradeEntity
    {
        /// <summary>
        /// 指定したノードの現在の取得回数を取得します。
        /// </summary>
        /// <param name="node">確認するノード。</param>
        /// <returns>取得回数。未取得の場合は0。</returns>
        public int GetCount(LevelUpgradeNode node)
        {
            return _acquiredUpgrades.TryGetValue(node, out int count) ? count : 0;
        }

        /// <summary>
        /// 指定したノードを1回取得したとして記録します。
        /// </summary>
        /// <param name="node">取得したノード。</param>
        public void Add(LevelUpgradeNode node)
        {
            if (_acquiredUpgrades.ContainsKey(node))
            {
                _acquiredUpgrades[node]++;
            }
            else
            {
                _acquiredUpgrades[node] = 1;
            }
        }

        private readonly Dictionary<LevelUpgradeNode, int> _acquiredUpgrades = new();
    }
}
