using Cryptos.Runtime.Entity.Ingame.System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    /// <summary>
    /// レベルアップノードのビューモデル。
    /// </summary>
    public readonly struct LevelUpgradeNodeViewModel
    {
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        /// <param name="node">ビューモデルがラップするノード。</param>
        public LevelUpgradeNodeViewModel(LevelUpgradeNode node, int currentLevel)
        {
            _node = node;
            _currentLevel = currentLevel;
        }

        /// <summary> ノード名。 </summary>
        public string NodeName => _node.NodeName;
        /// <summary> ノードのテクスチャ。 </summary>
        public Texture2D Texture => _node.Texture;
        /// <summary> ノードの説明。 </summary>
        public string Description => _node.Description;
        /// <summary> ノードの最大ランク。 </summary>
        public int MaxRank => _node.MaxStack;

        /// <summary>
        ///     このビューモデルがラップするノード。
        /// </summary>
        public LevelUpgradeNode LevelUpgradeNode => _node;
        public int CurrentLevel => _currentLevel;

        private readonly LevelUpgradeNode _node;
        private readonly int _currentLevel;
    }
}
