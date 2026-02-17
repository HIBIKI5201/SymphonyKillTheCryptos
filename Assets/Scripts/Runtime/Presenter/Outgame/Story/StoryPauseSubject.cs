using Cryptos.Runtime.Entity.Outgame.Story;

namespace Cryptos.Runtime.Presenter.OutGame.Story
{
    public class StoryPauseSubject : IPauseSubject
    {
        public StoryPauseSubject(params IPauseSubjectView[] subjectView)
        {
            _pauses = new bool[subjectView.Length];
            for (int i = 0; i < subjectView.Length; i++)
            {
                subjectView[i].OnPause += d => ChangePause(i, d);
            }
        }

        public bool IsPaused => _isPaused;

        private bool[] _pauses;
        private bool _isPaused; 

        private void ChangePause(int index, bool value)
        {
            _pauses[index] = value;
            for (int i = 0; i < _pauses.Length; i++)
            {
                if (_pauses[i])
                {
                    _isPaused = true;
                    return;
                }
            }

            _isPaused = false;
        }
    }
}
