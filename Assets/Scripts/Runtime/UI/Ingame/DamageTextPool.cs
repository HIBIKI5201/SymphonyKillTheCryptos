using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI
{
    public class DamageTextPool
    {
        public DamageTextPool(VisualElement root)
        {
            _pool = new(
                createFunc: () => new UIElementDamageText(),
                actionOnGet: t => root.Add(t),
                actionOnRelease: t => root.Remove(t)
                );
        }

        public async void ShowDamageText(float damage, Vector3 position, float duration = 1)
        {
            UIElementDamageText text = _pool.Get();
            text.SetData(damage, position);

            await Awaitable.WaitForSecondsAsync(duration);

            _pool.Release(text);
        }

        private ObjectPool<UIElementDamageText> _pool;
    }
}
