using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Outgame.Story;

namespace Cryptos.Runtime.UseCase.OutGame.Story
{
    public class ScenarioActionRepository : IScenarioActionRepository
    {
        public ScenarioActionRepository(
            ICameraRepository cameraRep,
            ICharacterRepository charaRep)
        {
            _cameraRep = cameraRep;
            _charaRep = charaRep;
        }

        public ICameraRepository CameraRepository => _cameraRep;
        public ICharacterRepository CharacterRepository => _charaRep;

        private readonly ICameraRepository _cameraRep;
        private readonly ICharacterRepository _charaRep;
    }
}
