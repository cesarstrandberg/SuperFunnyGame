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

    [Header("Debug")]
    public bool spawningEnabled = true;

    [Header("Whisky Inställningar")]
    public float yOffset = 0.05f;

    [Header("Timing")]
    public float timeBetweenWaves = 12f;
    public float delayBetweenSpawns = 8f;

    [Header("Status")]
    public int currentWave = 0;
    private int zombiesToSpawn;
    private int zombiesAlive = 0;
    private bool isSpawning = false;

    void Awake() { instance = this; }

    void Start() { StartCoroutine(NextWaveRoutine()); }

    IEnumerator NextWaveRoutine()
    {
        currentWave++;
        if (currentWaveDisplayText != null) currentWaveDisplayText.text = "WAVE: " + currentWave;

        RespawnWhisky();
        yield return new WaitForSeconds(timeBetweenWaves);

        zombiesToSpawn = Mathf.CeilToInt(currentWave / 1.5f);
        StartCoroutine(SpawnWave());
    }

     // Justera denna i Inspectorn (t.ex. 0.02 för att sänka mer)

    void RespawnWhisky()
    {
        GameObject[] oldWhiskies = GameObject.FindGameObjectsWithTag("Whisky");
        foreach (GameObject w in oldWhiskies) { if (w != null) Destroy(w); }

        foreach (Transform spot in whiskySpawnPoints)
        {
            if (spot != null && whiskyPrefab != null)
            {
                Vector3 spawnPos = spot.position;

                // RAYCAST: Skjut en osynlig stråle neråt för att hitta bordet
                RaycastHit hit;
                if (Physics.Raycast(spot.position, Vector3.down, out hit, 2f))
                {
                    // Om vi träffar något (bordet), sätt positionen precis på ytan + din lilla offset
                    spawnPos = hit.point + Vector3.up * yOffset;
                }

                Instantiate(whiskyPrefab, spawnPos, spot.rotation);
            }
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            // Kollar om vi får spawna just nu
            if (spawningEnabled)
            {
                SpawnZombie();
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
            else
            {
                // Om vi stängt av, vänta en sekund och kolla igen
                i--;
                yield return new WaitForSeconds(1f);
            }
        }
        isSpawning = false;
    }
    

    void SpawnZombie()
    {
        Transform selectedPoint = (Random.value < 0.8f) ? doorSpawnPoint : kitchenSpawnPoint;
        if (selectedPoint != null) Instantiate(zombiePrefab, selectedPoint.position, selectedPoint.rotation);
        zombiesAlive++;
    }

    public void EnemyDied()
    {
        zombiesAlive--;
        if (zombiesAlive <= 0 && !isSpawning)
        {
            if (GameUIHandler.instance != null) GameUIHandler.instance.ShowWaveComplete(currentWave);
            StartCoroutine(NextWaveRoutine());
        }
    }
}