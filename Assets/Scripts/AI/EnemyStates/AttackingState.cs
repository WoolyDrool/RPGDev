using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.AI
{ 
    public class AttackingState : AIBaseBehaviourState
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("I am attacking");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}


