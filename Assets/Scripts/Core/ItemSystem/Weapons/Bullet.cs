using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public double damage = 0;
    public float maxRange;
    Vector3 startingPosition;
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit enemy, " + damage.ToString());
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        Destroy(gameObject);
    }

    void Awake()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float distFromPlayer = Vector3.Distance(startingPosition, currentPosition);


        if(distFromPlayer > maxRange)
        {
            Destroy(gameObject);
        }
    }

}
