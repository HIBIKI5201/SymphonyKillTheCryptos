using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    public abstract class LevelUpgradeNodeBase : ScriptableObject
    {
        public string NodeName => _nodeName;
        public Texture2D Texture => _texture;

        public abstract void Execute();

        [SerializeField]
        private string _nodeName = "New Node";
        [SerializeField, TextArea]
        private string _description = "Description";

        [SerializeField]
        private Texture2D _texture;
    }
}
