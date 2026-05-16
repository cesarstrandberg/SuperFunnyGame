using UnityEngine;
using System.Collections;
using TMPro;
using Unity.Cinemachine; // Unity 6 Cinemachine

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("Referenser")]
    public GameObject zombiePrefab;
    public GameObject whiskyPrefab;
    public Transform[] zombieSpawnPoints;
    public Transform[] whiskySpawnPoints;
    public TextMeshProUGUI currentWaveDisplayText;

    [Header("Intro Text-slots (Ska vara avkryssade!)")]
    public TextMeshProUGUI welcomeTextUI;
    public TextMeshProUGUI companyTextUI;
    public TextMeshProUGUI killHimTextUI;

    [Header("Paus Text-slots (Ligger kvar 2 sek in i nästa våg!)")]
    public TextMeshProUGUI pause1TextUI;
    public TextMeshProUGUI pause2TextUI;
    public TextMeshProUGUI pause3TextUI;
    public TextMeshProUGUI pause4TextUI;
    public TextMeshProUGUI pause5TextUI;

    [Header("Intro Kamera & Ljud")]
    public GameObject zoomCamera;
    public AudioSource zombieSFXSource;
    public AudioClip zombieGrowlClip;

    [Header("NYTT: Zombie Spawn Varselljud (3D)")]
    public AudioClip zombieSpawnSubtleClip;   // Dra in ditt korta spawn-ljud här!
    [Range(0f, 1f)] public float zombieSpawnVolume = 0.4f; // Volym på spawn-ljudet

    [Header("Skräck-inställningar (Lampor)")]
    public Light kitchenLight;
    public Light ceilingLight;

    [Header("Skräck-inställningar (Musik)")]
    public AudioSource musicBoxSource;

    [Header("3D-Ljudkällor (Miljö)")]
    public AudioSource glassShatterSource;
    public AudioSource girlLaughSource;
    public AudioSource doorSlamSource;
    public AudioSource fuseBlowSource;

    [Header("Ljudfiler (Miljö)")]
    public AudioClip glassShatterClip;
    public AudioClip girlLaughClip;
    public AudioClip doorSlamClip;
    public AudioClip fuseBlowClip;

    [Header("Inställningar")]
    public bool spawningEnabled = true;
    public float yOffset = 0.25f; // ÄNDRAD: Höjt standardvärdet från 0.05 till 0.25 för att motverka bords-clipping!
    public float timeBetweenWaves = 3f;
    public float delayBetweenSpawns = 5f;

    [Header("Status")]
    public int currentWave = 1;
    private int zombiesToSpawn;
    private int zombiesAlive = 0;
    private bool isSpawning = false;

    void Awake()
    {
        instance = this;
        currentWave = 1;
    }

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

        // --- ENBART INTRO-LOGIKEN FÖR VÅG 1 ---
        if (currentWave == 1)
        {
            if (welcomeTextUI != null) welcomeTextUI.gameObject.SetActive(true);
            if (companyTextUI != null) companyTextUI.gameObject.SetActive(false);
            if (killHimTextUI != null) killHimTextUI.gameObject.SetActive(false);
            yield return new WaitForSeconds(7f);

            if (welcomeTextUI != null) welcomeTextUI.gameObject.SetActive(false);
            if (companyTextUI != null) companyTextUI.gameObject.SetActive(true);
            if (zombieSFXSource != null && zombieGrowlClip != null)
                zombieSFXSource.PlayOneShot(zombieGrowlClip);

            yield return new WaitForSeconds(3f);
            if (companyTextUI != null) companyTextUI.gameObject.SetActive(false);
        }
        // --- PAUSERNA MELLAN RONDERNA ---
        else
        {
            SetAllPauseTextsActive(false);
            yield return new WaitForSeconds(7f);

            int avklaradVag = currentWave - 1;

            if (avklaradVag == 1)
            {
                if (pause1TextUI != null) pause1TextUI.gameObject.SetActive(true);
                if (glassShatterSource != null && glassShatterClip != null)
                    glassShatterSource.PlayOneShot(glassShatterClip);

                yield return new WaitForSeconds(2f);

                if (girlLaughSource != null && girlLaughClip != null)
                    girlLaughSource.PlayOneShot(girlLaughClip);

                yield return new WaitForSeconds(8f);
            }
            else if (avklaradVag == 2)
            {
                yield return new WaitForSeconds(3f);
                if (pause2TextUI != null) pause2TextUI.gameObject.SetActive(true);

                if (doorSlamSource != null && doorSlamClip != null)
                    doorSlamSource.PlayOneShot(doorSlamClip);

                yield return new WaitForSeconds(5f);
            }
            else if (avklaradVag == 3)
            {
                if (pause3TextUI != null) pause3TextUI.gameObject.SetActive(true);
                if (kitchenLight != null) kitchenLight.intensity *= 0.4f;

                yield return StartCoroutine(SlowDownMusicBox(7f));

                if (musicBoxSource != null)
                {
                    musicBoxSource.pitch = 2.0f;
                    musicBoxSource.Play();
                }
            }
            else if (avklaradVag == 4)
            {
                if (pause4TextUI != null) pause4TextUI.gameObject.SetActive(true);
                if (kitchenLight != null) kitchenLight.enabled = false;
                if (ceilingLight != null) ceilingLight.intensity *= 0.4f;

                if (fuseBlowSource != null && fuseBlowClip != null)
                    fuseBlowSource.PlayOneShot(fuseBlowClip);

                yield return new WaitForSeconds(5f);
            }
            else if (avklaradVag == 5)
            {
                if (pause5TextUI != null) pause5TextUI.gameObject.SetActive(true);
                if (kitchenLight != null) kitchenLight.enabled = false;
                if (ceilingLight != null) ceilingLight.enabled = false;

                if (musicBoxSource != null)
                {
                    musicBoxSource.pitch = 3.0f;
                    if (!musicBoxSource.isPlaying) musicBoxSource.Play();
                }

                yield return new WaitForSeconds(5f);
            }
            else
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }

            StartCoroutine(TurnOffPauseTextDelayed(2f));
        }

        // --- SPAWN-LOGIKEN ---
        zombiesToSpawn = Mathf.CeilToInt(currentWave / 1.1f);
        if (zombiesToSpawn < 1) zombiesToSpawn = 1;

        isSpawning = true;
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            if (spawningEnabled)
            {
                GameObject nyssSpawnadZombie = null;

                if (currentWave == 1 && i == 0)
                {
                    nyssSpawnadZombie = SpawnZombie(1);
                    StartCoroutine(TriggerIntroZoom(nyssSpawnadZombie));
                }
                else
                {
                    nyssSpawnadZombie = SpawnZombie(-1);
                }

                if (i < zombiesToSpawn - 1)
                    yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
        isSpawning = false;
    }

    IEnumerator TurnOffPauseTextDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetAllPauseTextsActive(false);
    }

    void SetAllPauseTextsActive(bool state)
    {
        if (pause1TextUI != null) pause1TextUI.gameObject.SetActive(state);
        if (pause2TextUI != null) pause2TextUI.gameObject.SetActive(state);
        if (pause3TextUI != null) pause3TextUI.gameObject.SetActive(state);
        if (pause4TextUI != null) pause4TextUI.gameObject.SetActive(state);
        if (pause5TextUI != null) pause5TextUI.gameObject.SetActive(state);
    }

    IEnumerator SlowDownMusicBox(float duration)
    {
        if (musicBoxSource == null) yield break;

        float startPitch = musicBoxSource.pitch;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicBoxSource.pitch = Mathf.Lerp(startPitch, 0.0f, elapsed / duration);
            yield return null;
        }

        musicBoxSource.Stop();
    }

    IEnumerator TriggerIntroZoom(GameObject targetZombie)
    {
        if (killHimTextUI != null) killHimTextUI.gameObject.SetActive(true);

        if (zoomCamera != null)
        {
            CinemachineCamera vcam = zoomCamera.GetComponent<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.Follow = targetZombie.transform;
                vcam.LookAt = targetZombie.transform;
            }
            zoomCamera.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        if (zoomCamera != null) zoomCamera.SetActive(false);
        if (killHimTextUI != null) killHimTextUI.gameObject.SetActive(false);
    }

    GameObject SpawnZombie(int forcedIndex = -1)
    {
        if (zombieSpawnPoints.Length > 0)
        {
            int indexToUse = Random.Range(0, zombieSpawnPoints.Length);
            if (forcedIndex >= 0 && forcedIndex < zombieSpawnPoints.Length)
            {
                indexToUse = forcedIndex;
            }

            Transform selectedPoint = zombieSpawnPoints[indexToUse];
            if (selectedPoint != null)
            {
                ParticleSystem smokeVFX = selectedPoint.GetComponentInChildren<ParticleSystem>();
                if (smokeVFX != null)
                {
                    smokeVFX.Play();
                }

                // FIX: Spela upp ett subtilt 3D-varselljud exakt där zombien spawnar!
                if (zombieSpawnSubtleClip != null)
                {
                    AudioSource.PlayClipAtPoint(zombieSpawnSubtleClip, selectedPoint.position, zombieSpawnVolume);
                }

                GameObject zombie = Instantiate(zombiePrefab, selectedPoint.position, selectedPoint.rotation);
                zombiesAlive++;
                return zombie;
            }
        }
        return null;
    }

    public void EnemyDied()
    {
        zombiesAlive--;
        if (zombiesAlive <= 0 && !isSpawning)
        {
            currentWave++;
            UpdateHUD();

            if (GameUIHandler.instance != null)
                GameUIHandler.instance.ShowWaveComplete(currentWave);

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
                SpawnSingleWhisky(spot, Vector3.zero);

                if (currentWave == 4)
                {
                    SpawnSingleWhisky(spot, new Vector3(0.25f, 0f, 0.25f));
                    SpawnSingleWhisky(spot, new Vector3(-0.25f, 0f, -0.25f));
                }
            }
        }
    }

    void SpawnSingleWhisky(Transform spot, Vector3 offset)
    {
        Vector3 spawnPos = spot.position + offset;
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, 2f))
            spawnPos = hit.point + Vector3.up * yOffset;

        Instantiate(whiskyPrefab, spawnPos, spot.rotation);
    }
}