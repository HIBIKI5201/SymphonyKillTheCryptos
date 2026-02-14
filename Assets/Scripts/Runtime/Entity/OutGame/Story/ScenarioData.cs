namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public class ScenarioData
    {
        public ScenarioData(ScenarioNode[] nodes)
        {
            _scenarioNodes = nodes;
        }

        public ScenarioNode this[int index] => _scenarioNodes[index];

        private readonly ScenarioNode[] _scenarioNodes;
    }
}
