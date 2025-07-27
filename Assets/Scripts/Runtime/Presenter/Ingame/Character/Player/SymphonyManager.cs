using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Ingame.Character;
using SymphonyFrameWork.Attribute;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Player
{
    /// <summary>
    ///     プレイヤーのマネージャークラス
    /// </summary>
    public class SymphonyManager : MonoBehaviour
    {
        public PlayerCharacterEntity Entity => _entity;

        [SerializeField]
        private SymphonyData _symphonyData;

        private PlayerCharacterEntity _entity;

        private void Awake()
        {
            _entity = new PlayerCharacterEntity(_symphonyData);
        }
    }
}
