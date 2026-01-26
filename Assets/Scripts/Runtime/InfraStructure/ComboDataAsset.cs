using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.Ingame.DataAsset
{
    [CreateAssetMenu(fileName = nameof(ComboDataAsset),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(ComboDataAsset))]
    public class ComboDataAsset : ScriptableObject
    {
        public ComboData GetData()
        {
            return new(_comboDuration, _comboStackSpeeds);
        }

        [SerializeField]
        private float _comboDuration = 5;
        [SerializeField]
        private ComboStackSpeed[] _comboStackSpeeds =
            {
                new(3, 1.25f),
                new(5, 1.5f),
                new(10, 2f)
            };
    }
}
