using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifespan;
    void Update()
    {
        Destroy(this.gameObject, lifespan);
    }
}
