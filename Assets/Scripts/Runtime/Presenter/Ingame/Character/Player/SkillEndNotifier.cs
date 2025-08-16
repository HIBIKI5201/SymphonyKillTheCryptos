using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public class SkillEndNotifier : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {
            if (_receiver != null) return;

            if (!animator.TryGetComponent(out _receiver))
            {
                Debug.LogWarning("StateMachineBehaviourReceiver not found on the GameObject.");
            }
        }

        /// <summary>
        /// スキルの終了を通知するためのメソッド。
        /// </summary>
        /// <param name="animator">アニメーターコンポーネント</param>
        /// <param name="stateInfo">現在のアニメーション状態情報</param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _receiver?.NotifySkillEnd();
        }

        private SkillEndReceiver _receiver;
    }
}
