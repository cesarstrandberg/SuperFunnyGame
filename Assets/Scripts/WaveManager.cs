using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("Referenser")]
    public GameObject zombiePrefab;
    public GameObject whiskyPrefab;
    public Transform doorSpawnPoint;
    public Transform kitchenSpawnPoint;
    public Transform[] whiskySpawnPoints;
    public TextMeshProUGUI currentWaveDisplayText;

    [Header("Timing")]
    public float timeBetweenWaves = 12f;
    public float delayBetweenSpawns = 8f;

    [Header("Status")]
    public int currentWave = 0;
    private int zombiesToSpawn;
    private int zombiesAlive = 0;
    private bool isSpawning = false;

    void Awake() { instance = this; }

    void Start()
    {
        StartCoroutine(NextWaveRoutine());
    }

    IEnumerator NextWaveRoutine()
    {
        currentWave++;
        if (currentWaveDisplayText != null) currentWaveDisplayText.text = "WAVE: " + currentWave;

        RespawnWhisky();
        yield return new WaitForSeconds(timeBetweenWaves);

        zombiesToSpawn = Mathf.CeilToInt(currentWave / 1.5f);
        StartCoroutine(SpawnWave());
    }

    void RespawnWhisky()
    {
        GameObject[] oldWhisky = GameObject.FindGameObjectsWithTag("Whisky");
        foreach (GameObject w in oldWhisky) Destroy(w);

        foreach (Transform spot in whiskySpawnPoints)
        {
            if (whiskyPrefab != null) Instantiate(whiskyPrefab, spot.position, spot.rotation);
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
        isSpawning = false;
    }

    void SpawnZombie()
    {
        Transform selectedPoint = (Random.value < 0.8f) ? doorSpawnPoint : kitchenSpawnPoint;
        Instantiate(zombiePrefab, selectedPoint.position, selectedPoint.rotation);
        zombiesAlive++;
    }

    public void EnemyDied()
    {
        zombiesAlive--;

        if (zombiesAlive <= 0 && !isSpawning)
        {
            // FIXAD RAD: Nu ropar vi på den nya hanteraren
            if (GameUIHandler.instance != null)
            {
                GameUIHandler.instance.ShowWaveComplete(currentWave);
            }

            StartCoroutine(NextWaveRoutine());
        }
    }
}