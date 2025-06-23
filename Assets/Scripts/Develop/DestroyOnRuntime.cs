using UnityEngine;

namespace Cryptos.Runtime.Develop
{
    /// <summary>
    ///     起動時に破壊する
    /// </summary>
    public class DestroyOnRuntime : MonoBehaviour
    {
        private void Awake() => Destroy(gameObject);
    }
}
