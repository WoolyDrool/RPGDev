using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace RPGSystem.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIPathingManager : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;

        public Transform player;
        public float distanceToPlayer;

        public float pathingUpdateInterval = 0.5f;

        public bool targetPlayer = false;

        Vector3 directionToPlayer;

        public Vector3 curPos;

        Vector3 targetPoint;


        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            //SetNewDestination();
            //navMeshAgent.SetDestination(targetPoint);

            //EnablePathing();
            //StartCoroutine(MoveAgent());
        }

        IEnumerator Repath()
        {
            SetNewDestination();
            yield return new WaitForSeconds(pathingUpdateInterval);

            distanceToPlayer = Vector3.Distance(transform.position, player.position);

            curPos = transform.position;

            StartCoroutine(Repath());
            Debug.Log("Repathed");
        }

        void SetNewDestination()
        {
            if (targetPlayer)
            {
                targetPoint = player.localPosition;
                navMeshAgent.SetDestination(targetPoint);
            }
            else
            {
                targetPoint = transform.position;
            }
        }

        IEnumerator OverridePath(Vector3 newPosition, bool restart)
        {
            Debug.Log("Interrupting current path. Repositioning to " + newPosition.ToString());
            DisablePathing();

            navMeshAgent.SetDestination(newPosition);

            yield return new WaitUntil(() => Vector3.Distance(curPos, newPosition) <= 5);

            Debug.Log("Reached destination");
            
            if(restart) EnablePathing(); Debug.Log("Resuming");
        }

        public void SetOverrideDestination(Vector3 positionToMoveTo, bool resumeWhenFinished)
        {
            StartCoroutine(OverridePath(positionToMoveTo, resumeWhenFinished));
        }

        public void EnablePathing()
        {
            StartCoroutine(Repath());
        }

        public void DisablePathing()
        {
            StopCoroutine(Repath());
        }

        public void LookAtPlayer(bool rotate)
        {
            if(rotate) navMeshAgent.updateRotation = true;
            else navMeshAgent.updateRotation = false;
        }
    }
}
