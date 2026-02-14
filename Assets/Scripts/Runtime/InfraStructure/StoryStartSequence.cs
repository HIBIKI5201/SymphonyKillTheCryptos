using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.OutGame.Story;
using Cryptos.Runtime.UseCase.OutGame.Story;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.OutGame.Story
{
    public class StoryStartSequence : IGameInstaller
    {
        public ValueTask GameInitialize()
        {
            NovelSettingEntity setting = _settingAsset.Create();
            ScenarioActionRepository actionRepository =
                new ScenarioActionRepository(
                _cameraRep);

            return default;
        }

        [SerializeField]
        private NovelSettingAsset _settingAsset;
        [SerializeField]
        private StoryCameraRepository _cameraRep;
    }
}
