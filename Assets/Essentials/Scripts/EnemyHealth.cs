using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int HP = 100;

    public void TakeDamage(int damageAmount) 
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
