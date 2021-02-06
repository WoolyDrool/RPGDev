using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.AI
{
    public class AIBaseBehaviourState : StateMachineBehaviour
    {
        private AIPathingManager pathingManager;
        
        public AIPathingManager GetPathingManager(Animator animator)
        {
            if(pathingManager == null)
            {
                pathingManager = animator.GetComponentInParent<AIPathingManager>();
            }

            return pathingManager;
        }
    }

}

