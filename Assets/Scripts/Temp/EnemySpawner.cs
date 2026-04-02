using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnInterval = 10f; // Spawn every 10 seconds
    private bool isSpawning = false;

    void Start()
    {
        // Start spawning when the game starts
        StartCoroutine(SpawningRoutine());
    }

    IEnumerator SpawningRoutine()
    {
        // Set the spawning flag
        isSpawning = true;

        // Infinite loop for continuous spawning
        while (isSpawning)
        {
            // Spawn the enemy
            Instantiate(EnemyPrefab, transform.position, transform.rotation);

            // Wait for the spawn interval before spawning again
            yield return new WaitForSeconds(SpawnInterval);
        }
    }
}