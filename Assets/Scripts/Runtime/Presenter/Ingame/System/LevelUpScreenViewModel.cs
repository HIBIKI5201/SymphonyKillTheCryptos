namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public readonly struct LevelUpScreenViewModel
    {
        public LevelUpgradeNodeViewModel[] LevelUpgradeNodes { get; }
        public int CurrentPlayerLevel { get; }

        public LevelUpScreenViewModel(LevelUpgradeNodeViewModel[] levelUpgradeNodes, int currentPlayerLevel)
        {
            LevelUpgradeNodes = levelUpgradeNodes;
            CurrentPlayerLevel = currentPlayerLevel;
        }
    }
}
