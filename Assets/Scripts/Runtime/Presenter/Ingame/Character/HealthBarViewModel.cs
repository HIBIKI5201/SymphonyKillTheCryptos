using Cryptos.Runtime.Entity.Ingame.Character;
using System;
using System.Threading;
using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public readonly struct HealthBarViewModel
    {
        public HealthBarViewModel(CharacterEntity entity, Transform transform, CancellationToken token)
        {
            _entity = entity;
            _trackingTarget = transform;
            _token = token;
        }

        public event Action<float, float> OnHealthChaged
        {
            add => _entity.OnHealthChanged += value;
            remove => _entity.OnHealthChanged -= value;
        }

        public event Action OnDead
        {
            add => _entity.OnDead += value;
            remove => _entity.OnDead -= value;
        }

        public Transform TrackingTarget => _trackingTarget;
        public CancellationToken Token => _token;
        public float CurrentHealth => _entity.CurrentHealth;
        public float MaxHealth => _entity.HittableData.MaxHealth;

        private readonly CharacterEntity _entity;
        private readonly Transform _trackingTarget;
        private readonly CancellationToken _token;
    }
}
