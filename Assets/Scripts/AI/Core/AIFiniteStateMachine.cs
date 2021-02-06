using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPGSystem.AI
{
    [RequireComponent(typeof(Animator))]
    public class AIFiniteStateMachine : MonoBehaviour
    {
        Animator finiteStateMachine;

        private string currentState;

        void Awake()
        {
            finiteStateMachine = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void ChangeState(string newState)
        {
            if (currentState == newState) return;

            finiteStateMachine.Play(newState.ToString());

            currentState = newState;
        }
    }
}
