using Cryptos.Runtime.Framework;
using SymphonyFrameWork;
using SymphonyFrameWork.System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Cryptos.Runtime.Develop
{
    public class PlayerDev : MonoBehaviour, IInitializeAsync
    {
        private TextMeshProUGUI _text;

        Task IInitializeAsync.InitializeTask { get; set; }

        private void Start()
        {
            _text = FindAnyObjectByType<TextMeshProUGUI>();
        }

        async Task IInitializeAsync.InitializeAsync()
        {
            var buffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
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
