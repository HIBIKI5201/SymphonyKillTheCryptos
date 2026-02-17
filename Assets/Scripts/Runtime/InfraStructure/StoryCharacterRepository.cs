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
            _ = RotateAsync(target.transform, 0, 0);
            _showCharacters.Add(name, target);
        }


        public void Hide(string name)
        {
            if (!_showCharacters.TryGetValue(name, out GameObject c)) { return; }
            c.SetActive(false);
        }

        public async ValueTask Move(string name, Vector2 position, float duration)
        {
            if (_showCharacters.TryGetValue(name, out GameObject character))
            {
                await MoveAsync(character.transform, position, duration);
            }
        }

        public async ValueTask Rotate(string name, float degree, float duration)
        {
            if (_showCharacters.TryGetValue(name, out GameObject character))
            {
                await RotateAsync(character.transform, degree, duration);
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

        private async ValueTask RotateAsync(Transform target, float degree, float d)
        {
            // 入力が0の時に180°になる。
            degree = (degree + 180) % 360;

            Transform camera = Camera.main.transform;

            Vector3 camForward = camera.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 worldDir =
                Quaternion.AngleAxis(degree, Vector3.up) * camForward;

            worldDir.y = 0f;
            worldDir.Normalize();
            Quaternion yawRotation = Quaternion.LookRotation(worldDir);

            Vector3 currentEuler = target.eulerAngles;
            Quaternion finalRotation =
                Quaternion.Euler(currentEuler.x,
                                 yawRotation.eulerAngles.y,
                                 currentEuler.z);

            if (d <= 0f)
            {
                target.rotation = finalRotation;
                return;
            }

            await SymphonyTween.Tweening(
                target.rotation,
                q => target.rotation = q,
                finalRotation,
                d);
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
