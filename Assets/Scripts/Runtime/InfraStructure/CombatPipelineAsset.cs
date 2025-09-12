using Cryptos.Runtime.Framework;
using Cryptos.Runtime.UseCase.Ingame.CombatSystem;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure
{
    [CreateAssetMenu(menuName = CryptosPathConstant.ASSET_PATH + nameof(CombatPipelineAsset),
        fileName = nameof(CombatPipelineAsset))]
    public class CombatPipelineAsset : ScriptableObject
    {
        public ICombatHandler[] CombatHandler => _combatHandlers;

        [SerializeReference, SubclassSelector, Tooltip("戦闘処理のパイプライン")]
        private ICombatHandler[] _combatHandlers;
    }
}
