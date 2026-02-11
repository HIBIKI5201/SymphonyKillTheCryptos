using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.OutGame.Card;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure
{
    [CreateAssetMenu(fileName = nameof(RoleDataBase),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(RoleDataBase))]
    public class RoleDataBase : ScriptableObject
    {
        public RoleAsset[] RoleAssets => _roleAssets;

        [SerializeField]
        private RoleAsset[] _roleAssets;
    }
}
