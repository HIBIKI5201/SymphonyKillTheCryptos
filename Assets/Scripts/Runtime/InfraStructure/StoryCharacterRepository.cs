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

            GameObject target = null;
            for (int i = 0; i < _characters.Length; i++)
            {
                CharacterInfo chara = _characters[i];
                if (chara.Name == name)
                {
                    target = chara.Prefab;
                    break;
                }
            }

            target = Instantiate(target);
            target.transform.SetParent(transform);
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

        private Dictionary<string, GameObject> _showCharacters = new();

        private async ValueTask MoveAsync(Transform target, Vector2 pos, float d)
        {
            if (d <= 0) { target.position = pos; }

            await SymphonyTween.Tweening<Vector2>(target.position,
                p => target.position = p,
                pos, d);
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
