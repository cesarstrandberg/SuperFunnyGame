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

    [Header("Inställningar")]
    public bool spawningEnabled = true;
    public float yOffset = 0.05f;
    public float timeBetweenWaves = 3f;
    public float delayBetweenSpawns = 5f;

    [Header("Status")]
    public int currentWave = 1;
    private int zombiesToSpawn;
    private int zombiesAlive = 0;
    private bool isSpawning = false;

    void Awake() { instance = this; }

    void Start()
    {
        UpdateHUD();
        StartCoroutine(SpawnWaveRoutine());
    }

    void UpdateHUD()
    {
        if (currentWaveDisplayText != null) currentWaveDisplayText.text = "WAVE: " + currentWave;
    }

    IEnumerator SpawnWaveRoutine()
    {
        RespawnWhisky();
        yield return new WaitForSeconds(timeBetweenWaves);

        zombiesToSpawn = Mathf.CeilToInt(currentWave / 1.1f);
        if (zombiesToSpawn < 1) zombiesToSpawn = 1;

        isSpawning = true;
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            if (spawningEnabled)
            {
                SpawnZombie();
                if (i < zombiesToSpawn - 1)
                    yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
        isSpawning = false;
    }

    void SpawnZombie()
    {
        Transform selectedPoint = (Random.value < 0.8f) ? doorSpawnPoint : kitchenSpawnPoint;
        if (selectedPoint != null)
        {
            Instantiate(zombiePrefab, selectedPoint.position, selectedPoint.rotation);
            zombiesAlive++;
        }
    }

    public void EnemyDied()
    {
        zombiesAlive--;
        if (zombiesAlive <= 0 && !isSpawning)
        {
            // 1. VISA KORTET FÖRST (med nuvarande wave-nummer)
            if (GameUIHandler.instance != null)
                GameUIHandler.instance.ShowWaveComplete(currentWave);

            // 2. ÖKA SIFFRAN EFTERÅT (för nästa runda)
            currentWave++;
            UpdateHUD();
            StartCoroutine(SpawnWaveRoutine());
        }
    }

    void RespawnWhisky()
    {
        GameObject[] oldWhiskies = GameObject.FindGameObjectsWithTag("Whisky");
        foreach (GameObject w in oldWhiskies) { if (w != null) Destroy(w); }
        foreach (Transform spot in whiskySpawnPoints)
        {
            if (spot != null && whiskyPrefab != null)
            {
                Vector3 spawnPos = spot.position;
                RaycastHit hit;
                if (Physics.Raycast(spot.position, Vector3.down, out hit, 2f))
                    spawnPos = hit.point + Vector3.up * yOffset;
                Instantiate(whiskyPrefab, spawnPos, spot.rotation);
            }
        }
    }
}