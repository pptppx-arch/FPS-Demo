using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public int bulletDamage;

    public GameObject ImpactPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 position = contact.point + Vector3.up * 0.05f;
            Quaternion rotation = Quaternion.LookRotation(contact.normal);
            GameObject impactEffect = Instantiate(GlobalReference.Instance.ImpactPrefab, contact.point + contact.normal * 0.05f, Quaternion.LookRotation(contact.normal) * Quaternion.Euler(90, 0, 0));
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(bulletDamage);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerHealth>() != null)
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(bulletDamage);
            }
        }
        else
        {

        }
        Destroy(gameObject);
    }
}
