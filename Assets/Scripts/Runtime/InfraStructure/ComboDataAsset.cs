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
        private float _comboDuration;
        [SerializeField]
        private ComboStackSpeed[] _comboStackSpeeds;
    }
}
