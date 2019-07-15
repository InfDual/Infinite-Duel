using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duel.PlayerSystems
{
    public class AnimStateEventTrigger : StateMachineBehaviour
    {
        private PlayerMaster master;

        [SerializeField]
        private PlayerAnimationStateEventType eventType;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (master == null)
            {
                master = animator.GetComponent<PlayerMaster>();
            }

            master.InvokePlayerEvent(new PlayerAnimationStateEvent(eventType));
        }
    }
}