using Cryptos.Runtime.Entity.Ingame.System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.System
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
        public LevelUpgradeNodeViewModel(LevelUpgradeNode node)
        {
            _node = node;
            // TODO 現在レベルの情報を追加する。
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

        private readonly LevelUpgradeNode _node;
    }
}
