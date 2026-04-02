using UnityEngine;

public class BoxCollisionDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("yes");
            //do something
            Destroy(gameObject);
        }
    }
}
