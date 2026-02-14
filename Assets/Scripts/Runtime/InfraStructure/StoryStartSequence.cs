using Cryptos.Runtime.Entity.Outgame.Story;
using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.OutGame.Story;
using Cryptos.Runtime.UseCase.OutGame.Story;
using SymphonyFrameWork.System;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.OutGame.Story
{
    public class StoryStartSequence : IGameInstaller
    {
        public async ValueTask GameInitialize()
        {
            NovelSettingEntity setting = _settingAsset.Create();
            ScenarioDataEntity data = await ServiceLocator.GetInstanceAsync<ScenarioDataEntity>();

            ScenarioData scenario = _scenarioDataBase.GetScenarioData(data.PlayIndex);

            StoryPauseSubject ps = new StoryPauseSubject();
            StoryMessageViewModel vm = new(setting, ps);

            ScenarioActionRepository actionRepository =
                new ScenarioActionRepository(
                _cameraRep);
            ScenarioPlayer player = new(
                scenario,
                actionRepository,
                vm,
                ps);
        }

        [SerializeField]
        private NovelSettingAsset _settingAsset;
        [SerializeField]
        private StoryCameraRepository _cameraRep;
        [SerializeField]
        private ScenarioDataBase _scenarioDataBase;
    }
}
