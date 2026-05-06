using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;    // Dra in din Zombie-PFB här
    public Transform[] spawnPoints;    // En lista med platser där de kan dyka upp
    public float timeBetweenSpawns = 5f; // Hur ofta de spawnar
    public int maxZombies = 10;        // Max antal zombies samtidigt i rummet

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenSpawns)
        {
            // Kolla hur många zombies som lever just nu
            int currentZombies = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (currentZombies < maxZombies)
            {
                SpawnZombie();
            }

            timer = 0;
        }
    }

    void SpawnZombie()
    {
        if (spawnPoints.Length == 0) return;

        // Välj en slumpmässig plats från listan
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform sp = spawnPoints[randomIndex];

        // Skapa zombien
        Instantiate(zombiePrefab, sp.position, sp.rotation);
    }
}