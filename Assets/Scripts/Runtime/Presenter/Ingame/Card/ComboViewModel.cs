using Cryptos.Runtime.Entity.Ingame.Card;
using System;

namespace Cryptos.Runtime.Presenter.Ingame.Card
{
    public readonly struct ComboViewModel
    {
        public ComboViewModel(ComboEntity comboEntity)
        {
            _comboEntity = comboEntity;
        }

        public event Action<float, float> OnChangedTimer
        {
            add => _comboEntity.OnChangedTimer += value;
            remove => _comboEntity.OnChangedTimer -= value;
        }

        public event Action<int> OnChangedCounter
        {
            add => _comboEntity.OnChangedCounter += value;
            remove => _comboEntity.OnChangedCounter -= value;
        }

        public event Action OnComboReset
        {
            add => _comboEntity.OnComboReset += value;
            remove => _comboEntity.OnComboReset -= value;
        }

        private readonly ComboEntity _comboEntity;
    }
}
