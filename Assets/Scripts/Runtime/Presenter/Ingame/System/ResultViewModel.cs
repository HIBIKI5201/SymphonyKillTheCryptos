namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public readonly struct ResultViewModel
    {
        public ResultViewModel(
            int currentLevel,
            int wave,
            int time,
            int skillPoint
            )
        {
            _currentLevel = currentLevel;
            _wave = wave;
            _time = time;
            _skillPoint = skillPoint;
        }

        public int CurrentLevel => _currentLevel;
        public int Wave => _wave;
        public int Time => _time;
        public int SkillPoint => _skillPoint;

        private readonly int _currentLevel;
        private readonly int _wave;
        private readonly int _time;
        private readonly int _skillPoint;
    }
}
