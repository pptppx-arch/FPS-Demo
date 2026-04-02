using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static int health = 100;
    public void TakeDamage(int enemyDamage)
    {
        health -= enemyDamage;
        if (health <= 0 )
        {
            //show you died
            //Destroy(gameObject);
        }
    }
}
