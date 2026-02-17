using Cryptos.Runtime.Entity.Outgame.Story;
using SymphonyFrameWork.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure
{
    public class StoryCharacterRepository : MonoBehaviour, ICharacterRepository
    {
        public void Show(string name)
        {
            if (_showCharacters.TryGetValue(name, out GameObject c))
            {
                c.SetActive(true);
                return;
            }

            GameObject prefab = null;
            for (int i = 0; i < _characters.Length; i++)
            {
                CharacterInfo chara = _characters[i];
                if (chara.Name == name)
                {
                    prefab = chara.Prefab;
                    break;
                }
            }

            GameObject target = Instantiate(prefab);
            target.transform.SetParent(transform);
            Rotate(target.transform, Vector3.back);
            _showCharacters.Add(name, target);
        }


        public void Hide(string name)
        {
            if (!_showCharacters.TryGetValue(name, out GameObject c)) { return; }
            c.SetActive(false);
        }

        public async void Move(string name, Vector2 position, float duration)
        {
            if (_showCharacters.TryGetValue(name, out GameObject character))
            {
                await MoveAsync(character.transform, position, duration);
            }
        }

        [SerializeField]
        private CharacterInfo[] _characters;
        [SerializeField]
        private float _zDistance = 5;

        private Dictionary<string, GameObject> _showCharacters = new();

        private async ValueTask MoveAsync(Transform target, Vector2 pos, float d)
        {
            // カメラからの相対座標にする。
            Transform camera = Camera.main.transform;
            Vector3 targetPos = camera.TransformPoint(new Vector3(pos.x, pos.y, _zDistance));
            
            if (d <= 0)
            { 
                target.position = targetPos;
                return;
            }

            await SymphonyTween.Tweening(target.position,
                p => target.position = p,
                targetPos, d);
        }

        private void Rotate(Transform target, Vector3 dir)
        {
            Transform camera = Camera.main.transform;
            Vector3 targetDir = camera.TransformDirection(dir);
            targetDir.y = 0f;
            targetDir.Normalize();

            target.rotation = Quaternion.LookRotation(targetDir);
        }

        [Serializable]
        private struct CharacterInfo
        {
            public string Name => _name;
            public GameObject Prefab => _prefab;

            [SerializeField]
            private string _name;
            [SerializeField]
            private GameObject _prefab;
        }
    }
}
