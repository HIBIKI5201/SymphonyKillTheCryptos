namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public class ScenarioDataEntity
    {
        public ScenarioDataEntity(int playIndex)
        {
            _playIndex = playIndex;
        }

        public int PlayIndex => _playIndex;

        private readonly int _playIndex;
    }
}
