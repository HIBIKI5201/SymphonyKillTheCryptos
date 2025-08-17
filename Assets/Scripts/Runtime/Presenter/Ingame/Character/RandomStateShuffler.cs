using UnityEngine;

namespace Cryptos.Runtime.Presenter
{
    public class RandomStateShuffler : StateMachineBehaviour
    {
        [SerializeField]
        private string _stateName;

        [SerializeField, Min(1)]
        private int _range = 1;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            int index = Random.Range(0, _range);
            animator.SetInteger(_stateName, index);
        }
    }
}
