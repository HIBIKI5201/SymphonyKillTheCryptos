using System;
using SymphonyFrameWork.Debugger;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cryptos.Runtime.Framework
{
    /// <summary>
    ///     入力をバッファリングするクラス
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class InputBuffer : MonoBehaviour
    {
        public event Action<char> OnAlphabetKeyPressed;
        public event Action<int> OnNumericKeyPressed;

        private const string NUMRIC_PREFIX = "Digit";

        private PlayerInput _input;

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
            OnAlphabetKeyPressed += DebugLogAlphabet;
            OnNumericKeyPressed += DebugLogNumric;
        }

        private void Update()
        {
            ReadKey();
        }

        /// <summary>
        ///     新たに押されたキーがあればイベントを発行する
        /// </summary>
        private void ReadKey()
        {
            if (Keyboard.current == null) return;

            foreach (var key in Keyboard.current.allKeys)
            {
                if (key == null) continue;
                
                if (key.wasPressedThisFrame) //押されているか判定
                {
                    string keyName = key.keyCode.ToString();

                    if (keyName.Length == 1) //アルファベットのみ
                    {
                        OnAlphabetKeyPressed?.Invoke(keyName[0]);
                    }

                    if (keyName.StartsWith(NUMRIC_PREFIX)) //Digitキーのみ
                    {
                        //キーコードから数値部分を取り出す
                        string numberString = keyName.Substring(NUMRIC_PREFIX.Length, 1);
                        int number = int.Parse(numberString);

                        OnNumericKeyPressed?.Invoke(number);
                    }
                }
            }
        }
        
        private void DebugLogAlphabet(char key)
        {
            Debug.Log($"pressed {key}");
        }

        private void DebugLogNumric(int  num)
        {
            Debug.Log($"pressed {num}");
        }
    }
}