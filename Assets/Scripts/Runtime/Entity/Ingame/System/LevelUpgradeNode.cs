using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    [CreateAssetMenu(fileName = nameof(LevelUpgradeNode), menuName = "Cryptos/" + nameof(LevelUpgradeNode), order = 0)]
    public class LevelUpgradeNode : ScriptableObject
    {
        public string NodeName => _nodeName;
        public string Description => _description;
        public Texture2D Texture => _texture;
        public ILevelUpgradeEffect[] Effects => _effects;

        [SerializeField]
        private string _nodeName = "New Node";
        [SerializeField, TextArea]
        private string _description = "Description";

        [SerializeField]
        private Texture2D _texture;

        [SerializeReference, SymphonySubclassSelector]
        private ILevelUpgradeEffect[] _effects;
    }
}
