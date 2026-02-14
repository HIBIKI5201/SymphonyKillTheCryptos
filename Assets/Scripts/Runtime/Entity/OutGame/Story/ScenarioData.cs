namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public class ScenarioData
    {
        public ScenarioData(ScenarioNode[] nodes)
        {
            _scenarioNodes = nodes;
        }

        public ScenarioNode this[int index] => _scenarioNodes[index];
        public int Length => _scenarioNodes.Length;

        private readonly ScenarioNode[] _scenarioNodes;
    }
}
