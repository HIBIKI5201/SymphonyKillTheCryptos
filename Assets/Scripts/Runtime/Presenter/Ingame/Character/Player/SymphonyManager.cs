using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyManager : MonoBehaviour
    {
        public CharacterEntity<SymphonyData> Entity => _entity;

        [SerializeField]
        private SymphonyData _symphonyData;

        [SerializeField]
        private CharacterEntity<SymphonyData> _entity;

        private void Awake()
        {
            _entity = new CharacterEntity<SymphonyData>(_symphonyData);
        }
    }
}
