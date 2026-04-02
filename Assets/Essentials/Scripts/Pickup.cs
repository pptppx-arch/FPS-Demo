using UnityEngine;

public class Pickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaycastFiring playerShoot = other.GetComponentInChildren<RaycastFiring>();
            playerShoot.TotalBullets += playerShoot.magazineSize;
            Destroy(gameObject);
        }
    }
}
