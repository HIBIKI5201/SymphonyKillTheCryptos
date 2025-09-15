using System;
using UnityEngine;

namespace Cryptos.Runtime.UI.Ingame.Character.Player
{
    public class SkillEndReceiver : MonoBehaviour
    {
        public event Action OnSkillEnd;

        public void NotifySkillEnd()
        {
            OnSkillEnd?.Invoke();
        }
    }
}
