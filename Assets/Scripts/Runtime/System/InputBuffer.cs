using System;
using SymphonyFrameWork.Debugger;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cryptos.Runtime.System
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputBuffer : MonoBehaviour
    {
        PlayerInput _input;

        public event Action<char> KeyPressed;

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
            _input.CheckComponentNull();

            if (_input)
            {
                _input.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            }
        }

        private void Start()
        {
            KeyPressed += c => Debug.Log($"pressed {c}");
        }

        private void Update()
        {
            ReadKey();
        }

        private void ReadKey()
        {
            if (Keyboard.current == null) return;

            foreach (var key in Keyboard.current.allKeys)
            {
                if (key == null) continue;
                
                if (key.wasPressedThisFrame)
                {
                    string keyName = key.keyCode.ToString();

                    if (keyName.Length == 1) //アルファベット以外を除外
                    {
                        KeyPressed?.Invoke(keyName[0]);
                    }
                }
            }
        }
    }
}