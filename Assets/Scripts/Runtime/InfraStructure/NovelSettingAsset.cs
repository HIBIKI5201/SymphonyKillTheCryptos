using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.OutGame.Story
{
    [CreateAssetMenu(fileName = nameof(NovelSettingAsset),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(NovelSettingAsset))]
    public class NovelSettingAsset : ScriptableObject
    {
        public NovelSettingEntity Create() 
        {
            return new(
                _textSpeed);
        }

        [SerializeField, Min(1)]
        private float _textSpeed = 30f;
    }
}
