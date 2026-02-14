using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.OutGame.Story
{
    [CreateAssetMenu(fileName = nameof(ScenarioDataAsset),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(ScenarioDataAsset))]
    public class ScenarioDataAsset : ScriptableObject
    {
        public ScenarioData GetDataBase()
        {
            return new(_scenarioNodes);
        }

        [SerializeField]
        private ScenarioNode[] _scenarioNodes;
    }
}
