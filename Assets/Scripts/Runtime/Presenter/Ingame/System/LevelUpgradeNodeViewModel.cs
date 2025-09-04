using Cryptos.Runtime.Entity.Ingame.System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.System
{
    public readonly struct LevelUpgradeNodeViewModel
    {
        public LevelUpgradeNodeViewModel(LevelUpgradeNode node)
        {
            _node = node;
        }

        public string NodeName => _node.NodeName;
        public Texture2D Texture => _node.Texture;
        public string Description => _node.Description;

        private readonly LevelUpgradeNode _node;
    }
}
