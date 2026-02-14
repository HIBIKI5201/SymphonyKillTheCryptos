using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.Framework;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.OutGame.Story
{
    [CreateAssetMenu(fileName = nameof(ScenarioDataBase),
        menuName = CryptosPathConstant.ASSET_PATH + nameof(ScenarioDataBase))]
    public class ScenarioDataBase : ScriptableObject
    {
        public ScenarioData GetScenarioData(int index)
        {
            ScenarioData data = _assets[index].GetDataBase();
            return data;
        }

        [SerializeField]
        private ScenarioDataAsset[] _assets;
    }
}
