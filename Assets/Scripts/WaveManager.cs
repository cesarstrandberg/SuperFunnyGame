using UnityEngine;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("Referenser")]
    public GameObject zombiePrefab;
    public GameObject whiskyPrefab; // Dra in din J&B-prefab här
    public Transform doorSpawnPoint;
    public Transform kitchenSpawnPoint;
    public Transform[] whiskySpawnPoints; // Skapa tomma objekt i rummet för där spriten ska stå
    public TextMeshProUGUI waveText;

    [Header("Timing")]
    public float timeBetweenWaves = 12f;   // Ännu längre paus för andrum
    public float delayBetweenSpawns = 8f;  // Längre paus mellan zombies

    [Header("Audio")]
    public AudioSource playerVoiceSource; // Dra in spelarens AudioSource (för citat)
    public AudioClip waveClearQuote;      // Dra in citatet som ska spelas efter vågen

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

        // MJUKARE START: 
        // Våg 1: 1 st, Våg 2: 1 st, Våg 3: 2 st, Våg 4: 2 st, Våg 5: 3 st...
        zombiesToSpawn = Mathf.CeilToInt(currentWave / 1.5f);

        if (waveText != null) waveText.text = "WAVE " + currentWave;

        // --- RESPRAWNA WHISKY ---
        RespawnWhisky();

        yield return new WaitForSeconds(timeBetweenWaves);

        StartCoroutine(SpawnWave());
    }

    void RespawnWhisky()
    {
        // Ta bort gamla flaskor om de finns kvar (valfritt)
        GameObject[] oldWhisky = GameObject.FindGameObjectsWithTag("Whisky"); // Se till att din prefab har taggen "Whisky"
        foreach (GameObject w in oldWhisky) Destroy(w);

        // Skapa nya på alla platser
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
            // VÅGEN ÄR SLUT: Spela citat
            if (playerVoiceSource != null && waveClearQuote != null)
            {
                playerVoiceSource.PlayOneShot(waveClearQuote);
            }

            Debug.Log("Våg rensad! Patrick firar med en Scotch.");
            StartCoroutine(NextWaveRoutine());
        }
    }
}