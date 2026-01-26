using System;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Player
{
    public interface ISymphonyAnimeManager
    {
        public event Action<int> OnSkillTriggered;
        public event Action OnSkillEnded;

        public bool IsAttacking { get; }

        public void SetDirX(float value);
        public void SetDirY(float value);
        public void SetVelocity(float value);
        public void SetSprint(bool value);
        public void ActiveSkill(int index);
        public void ChangeSpeed(float value);
    }
}
