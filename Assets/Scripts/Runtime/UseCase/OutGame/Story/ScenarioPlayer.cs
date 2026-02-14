using Cryptos.Runtime.Entity.Outgame.Story;

namespace Cryptos.Runtime.UseCase.OutGame.Story
{
    public class ScenarioPlayer
    {
        public ScenarioPlayer(ScenarioData scenarioData)
        {
            _scenarioData = scenarioData;
        }

        private readonly ScenarioData _scenarioData;
    }
}
