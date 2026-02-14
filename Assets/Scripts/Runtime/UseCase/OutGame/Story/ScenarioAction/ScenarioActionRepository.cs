using Cryptos.Runtime.Entity.Outgame.Story;

namespace Cryptos.Runtime.UseCase.OutGame.Story
{
    public class ScenarioActionRepository : IScenarioActionRepository
    {
        public ScenarioActionRepository(
            ICameraRepository cameraRep)
        {
            _cameraRep = cameraRep;
        }

        public ICameraRepository CameraRepository => _cameraRep;

        private readonly ICameraRepository _cameraRep;
    }
}
