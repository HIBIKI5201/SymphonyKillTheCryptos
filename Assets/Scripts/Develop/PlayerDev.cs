using Cryptos.Runtime.System;
using SymphonyFrameWork.Attribute;
using SymphonyFrameWork.System;
using TMPro;
using UnityEngine;

namespace Cryptos.Runtime.Ingame
{
    public class PlayerDev : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        
        private void Start()
        {
            _text = FindAnyObjectByType<TextMeshProUGUI>();

            var buffer = ServiceLocator.GetInstance<InputBuffer>();
            buffer.OnAlphabetKeyPressed += OnPressOnKey;
        }

        private void OnPressOnKey(char key)
        {
            string text = _text.text;

            if (char.ToUpper(text[0]) == key)
            {
                text = text.Substring(1);
            }
            
            _text.text = text;
        }
    }
}
