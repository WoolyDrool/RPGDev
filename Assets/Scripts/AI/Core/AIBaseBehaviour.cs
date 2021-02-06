using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGSystem.AI
{
    public class AIBaseBehaviour : MonoBehaviour
    {
        AIFiniteStateMachine stateMachine;

        AIPathingManager pathingManager;

        public float behaviourPingInterval = 1f;

        public bool canMove = false;

        const string ENEMY_IDLE = "IDLE";
        const string ENEMY_WALK = "WALK";
        const string ENEMY_ATTACK = "ATTACK";

        private void Awake()
        {
            stateMachine = GetComponent<AIFiniteStateMachine>();
            pathingManager = GetComponent<AIPathingManager>();
            StartCoroutine(Ping());
        }

        public virtual void InitializeAgent()
        {
            
        }
        public virtual void DiscardAgent()
        {

        }

        public virtual IEnumerator Ping()
        {
            //This is the update function, essentially
            //Used to avoid overloading the game with AI calls
            yield return new WaitForSeconds(behaviourPingInterval);
            StartCoroutine(Ping());
            
            /*if(pathingManager.distanceToPlayer < 16)
            {
                ChangeState(ENEMY_WALK);    
            }
            if (pathingManager.distanceToPlayer < 4)
            {
                ChangeState(ENEMY_ATTACK);
            }*/ 
        }
    }
}
