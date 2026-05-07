using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public float timeBetweenSpawns = 5f;
    public int maxZombies = 3; // Ändra till 3 i Inspectorn
    public int currentLivingEnemies = 0;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenSpawns)
        {
            // Vi kollar på vår egen räknare istället för att leta efter Tags
            if (currentLivingEnemies < maxZombies)
            {
                SpawnZombie();
            }

            timer = 0;
        }
    }

    public void EnemyDied()
    {
        currentLivingEnemies--;

        // Säkerhetskoll så den aldrig blir minus
        if (currentLivingEnemies < 0) currentLivingEnemies = 0;
    }

    void SpawnZombie()
    {
        if (spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform sp = spawnPoints[randomIndex];

        Instantiate(zombiePrefab, sp.position, sp.rotation);

        // VIKTIGT: Vi ökar räknaren här!
        currentLivingEnemies++;
    }
}