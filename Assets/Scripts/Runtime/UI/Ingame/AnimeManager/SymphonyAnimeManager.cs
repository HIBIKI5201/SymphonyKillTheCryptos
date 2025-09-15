using CriWare;
using System;
using UnityEngine;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using System.Text.RegularExpressions;
using System.Linq;

namespace Cryptos.Runtime.UI.Ingame.Character.Player
{
    /// <summary>
    ///     プレイヤーのアニメーションを管理するクラス
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(SkillEndReceiver))]
    public class SymphonyAnimeManager : MonoBehaviour, ISymphonyAnimeManager
    {
        public event Action<int> OnSkillTriggered;
        public event Action OnSkillEnded;

        public void SetDirX(float value) => _animator.SetFloat(_animatorHash.DirXHash, value);
        public void SetDirY(float value) => _animator.SetFloat(_animatorHash.DirYHash, value);
        public void SetVelocity(float value) => _animator.SetFloat(_animatorHash.VelocityHash, value);
        public void SetSprint(bool value) => _animator.SetBool(_animatorHash.SprintHash, value);

        public void ActiveSkill(int index)
        {
            _animator.SetInteger(_animatorHash.SkillIndexHash, index);
            _animator.SetTrigger(_animatorHash.SkillTriggerHash);
        }

        [SerializeField]
        private string _dirXParam = "DirX";
        [SerializeField]
        private string _dirYParam = "DirY";
        [SerializeField]
        private string _velocityParam = "Velocity";
        [SerializeField]
        private string _sprintParam = "Sprint";
        [SerializeField]
        private string _skillIndexParam = "SkillIndex";
        [SerializeField]
        private string _skillTriggerParam = "SkillTrigger";

        [Space]

        [SerializeField]
        private FirearmInfo[] _firearms;

        private Animator _animator;
        private SymphonyAnimatorHash _animatorHash;

        private CriAtomSource _atomSource;

        private void OnValidate()
        {
            _animatorHash = new SymphonyAnimatorHash(
                _dirXParam, _dirYParam,
                _velocityParam, _sprintParam,
                _skillIndexParam, _skillTriggerParam
            );
        }

        private void Awake()
        {
            if (!TryGetComponent(out _animator))
            {
                Debug.LogError("Animator component is not found on the GameObject.");
            }

            if (!TryGetComponent(out _atomSource))
            {
                Debug.LogError("CRI Atom Source is not found");
            }


            if (TryGetComponent(out SkillEndReceiver skillEndReceiver))
            {
                skillEndReceiver.OnSkillEnd += EndSkill;

                destroyCancellationToken.Register(() =>
                {
                    if (skillEndReceiver != null)
                    {
                        Debug.Log("Unsubscribing from OnSkillEnd event in SymphonyAnimeManager.");
                        skillEndReceiver.OnSkillEnd -= EndSkill;
                    }
                });
            }
            else
            {
                Debug.LogError("SkillEndReceiver component is not found on the GameObject.");
                return;
            }
        }

        private void TriggeredSkill(string info)
        {
            const string NumberPattern = @"\d+"; // 1桁以上の数字
            Match numberMatch = Regex.Match(info, NumberPattern);
            if (numberMatch.Success)
            {
                Debug.Log($"Skill animation event{numberMatch} triggered");
                int number = int.Parse(numberMatch.Value);
                OnSkillTriggered?.Invoke(number);
            }

            const string KIND_PATTERN = @"\[(.*?)\]"; //種類の文字列
            Match kindMatch = Regex.Match(info, KIND_PATTERN);
            if (kindMatch.Success)
            {
                string kind = kindMatch.Groups[1].Value; //最初にマッチしたグループ
                TriggerFirearm(kind); 
            }
        }

        private void TriggerFirearm(string kind)
        {
            kind = kind.ToLower();
            FirearmAnimeManager firearm =
                _firearms.FirstOrDefault(f => f.Kind.ToLower() == kind).AnimeManager;

            if (firearm == null)
            {
                Debug.LogError($"{kind} のfirearmがアサインされていません。");
                return;
            }

            firearm.Fire();
        }

        private void EndSkill()
        {
            Debug.Log("Skill animation ended");
            OnSkillEnded?.Invoke();
        }

        /// <summary>
        ///     ハッシュ値を保存する構造体。
        /// </summary>
        private readonly struct SymphonyAnimatorHash
        {
            public SymphonyAnimatorHash(string dirX, string dirY,
                string velocity, string sprint,
                string skillIndex, string skillTrigger)
            {
                _dirXHash = Animator.StringToHash(dirX);
                _dirYHash = Animator.StringToHash(dirY);
                _velocityHash = Animator.StringToHash(velocity);
                _sprintHash = Animator.StringToHash(sprint);
                _SkillIndexHash = Animator.StringToHash(skillIndex);
                _skillTriggerHash = Animator.StringToHash(skillTrigger);
            }

            public int DirXHash => _dirXHash;
            public int DirYHash => _dirYHash;
            public int VelocityHash => _velocityHash;
            public int SprintHash => _sprintHash;
            public int SkillIndexHash => _SkillIndexHash;
            public int SkillTriggerHash => _skillTriggerHash;

            private readonly int _dirXHash;
            private readonly int _dirYHash;
            private readonly int _velocityHash;
            private readonly int _sprintHash;
            private readonly int _SkillIndexHash;
            private readonly int _skillTriggerHash;
        }

        [Serializable]
        private struct FirearmInfo
        {
            public string Kind => _kind;
            public FirearmAnimeManager AnimeManager => _animeManager;

            [SerializeField]
            private string _kind;
            [SerializeField]
            private FirearmAnimeManager _animeManager;
        }
    }
}
