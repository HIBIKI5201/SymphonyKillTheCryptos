using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Presenter
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
