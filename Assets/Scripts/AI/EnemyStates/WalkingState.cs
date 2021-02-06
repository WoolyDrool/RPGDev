using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.AI
{
    public class WalkingState : AIBaseBehaviourState
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPathingManager(animator).navMeshAgent.SetDestination(GetPathingManager(animator).player.position);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
    }
}


