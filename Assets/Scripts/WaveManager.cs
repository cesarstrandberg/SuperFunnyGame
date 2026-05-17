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

    [Header("Paus Text-slots")]
    public TextMeshProUGUI pause1TextUI;
    public TextMeshProUGUI pause2TextUI;
    public TextMeshProUGUI pause3TextUI;
    public TextMeshProUGUI pause4TextUI;    // Skriv "Just a blown fuse..." här!
    public TextMeshProUGUI pause5TextUI;    // Skriv "RUN TO THE DOOR!" här!

    [Header("Intro Kamera & Ljud")]
    public GameObject zoomCamera;
    public AudioSource zombieSFXSource;
    public AudioClip zombieGrowlClip;       // Det feta intro-vrålet

    [Header("Zombie Spawn Varselljud (3D)")]
    public AudioClip zombieSpawnSubtleClip;
    [Range(0f, 1f)] public float zombieSpawnVolume = 0.4f;
    public AudioClip zombieDefaultRosselClip; // Lågt rossel som loopar på zombierna!

    [Header("Skräck-inställningar (Lampor)")]
    public Light kitchenLight;
    public Light ceilingLight;
    public Light doorLight;                   // Lampan vid ytterdörren

    [Header("Skräck-inställningar (Musik)")]
    public AudioSource musicBoxSource;

    [Header("Spelar-referens för Hälso-boost")]
    public GameObject playerAmateur;

    [Header("3D-Ljudkällor")]
    public AudioSource glassShatterSource;
    public AudioSource girlLaughSource;
    public AudioSource doorSlamSource;
    public AudioSource fuseBlowSource;

    [Header("Ljudfiler")]
    public AudioClip glassShatterClip;
    public AudioClip girlLaughClip;
    public AudioClip doorSlamClip;
    public AudioClip fuseBlowClip;

    [Header("Inställningar")]
    public bool spawningEnabled = true;
    public float yOffset = 0.25f;
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
        if (doorLight != null) doorLight.enabled = false;
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

            // FIX: Hittar köksspawnen och spelar growl-ljudet i 3D därifrån innan zombien skapas!
            Transform kitchenSpot = FindSpawnPointByName("KitchenSpawn");
            Vector3 soundPos = (kitchenSpot != null) ? kitchenSpot.position : Vector3.zero;

            if (zombieGrowlClip != null)
            {
                AudioSource.PlayClipAtPoint(zombieGrowlClip, soundPos, zombieSpawnVolume * 1.2f);
            }

            yield return new WaitForSeconds(3f);
            if (companyTextUI != null) companyTextUI.gameObject.SetActive(false);
        }
        // --- PAUSERNA MELLAN RONDERNA ---
        else
        {
            SetAllPauseTextsActive(false);

            int avklaradVag = currentWave - 1;

            if (avklaradVag == 5)
            {
                Debug.Log("Våg 5 klar! INSTANT BLACKOUT! RUN TO THE DOOR!");
                if (pause5TextUI != null) pause5TextUI.gameObject.SetActive(true);

                if (kitchenLight != null) kitchenLight.enabled = false;
                if (ceilingLight != null) ceilingLight.enabled = false;
                if (doorLight != null) doorLight.enabled = true;

                if (musicBoxSource != null)
                {
                    musicBoxSource.pitch = 3.0f;
                    if (!musicBoxSource.isPlaying) musicBoxSource.Play();
                }
            }
            else
            {
                yield return new WaitForSeconds(7f);

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
                    Debug.Log("Våg 4 klar! Ger PlayerAmateur +50 HP inför Våg 5.");
                    GivePlayerHealthBoost(50);

                    if (pause4TextUI != null) pause4TextUI.gameObject.SetActive(true);
                    if (kitchenLight != null) kitchenLight.enabled = false;
                    if (ceilingLight != null) ceilingLight.intensity *= 0.4f;

                    if (fuseBlowSource != null && fuseBlowClip != null)
                        fuseBlowSource.PlayOneShot(fuseBlowClip);

                    yield return new WaitForSeconds(5f);
                }
            }

            StartCoroutine(TurnOffPauseTextDelayed(2f));
        }

        // --- SPAWN-LOGIKEN ---
        if (currentWave == 6)
        {
            Debug.Log("VÅG 6: Startar specialspawns direkt vid blackout!");
            yield return StartCoroutine(SpawnFinalEscapeWaveRoutine());
        }
        else
        {
            zombiesToSpawn = Mathf.CeilToInt(currentWave / 1.1f);
            if (zombiesToSpawn < 1) zombiesToSpawn = 1;

            isSpawning = true;
            for (int i = 0; i < zombiesToSpawn; i++)
            {
                if (spawningEnabled)
                {
                    GameObject nyssSpawnadZombie = (currentWave == 1 && i == 0) ? SpawnZombie(1) : SpawnZombie(-1);
                    if (currentWave == 1 && i == 0 && nyssSpawnadZombie != null)
                    {
                        StartCoroutine(TriggerIntroZoom(nyssSpawnadZombie));
                    }

                    if (i < zombiesToSpawn - 1)
                        yield return new WaitForSeconds(delayBetweenSpawns);
                }
            }
            isSpawning = false;
        }
    }

    IEnumerator SpawnFinalEscapeWaveRoutine()
    {
        isSpawning = true;

        Transform kitchenSpot = FindSpawnPointByName("KitchenSpawn");
        Transform chairSpot = FindSpawnPointByName("CornerChairSpawn");

        for (int i = 0; i < 3; i++)
        {
            if (kitchenSpot != null) SpawnZombieAtSpecificTransform(kitchenSpot);
            yield return new WaitForSeconds(3.0f);
        }

        for (int i = 0; i < 2; i++)
        {
            if (chairSpot != null) SpawnZombieAtSpecificTransform(chairSpot);
            yield return new WaitForSeconds(1.5f);
        }

        isSpawning = false;
    }

    Transform FindSpawnPointByName(string nameToFind)
    {
        foreach (Transform spot in zombieSpawnPoints)
        {
            if (spot != null && spot.name == nameToFind) return spot;
        }
        return null;
    }

    void SpawnZombieAtSpecificTransform(Transform targetSpot)
    {
        ParticleSystem smokeVFX = targetSpot.GetComponentInChildren<ParticleSystem>();
        if (smokeVFX != null) smokeVFX.Play();

        // 1. Spela det korta, subtila spawn-varselljudet (3D)
        if (zombieSpawnSubtleClip != null)
            AudioSource.PlayClipAtPoint(zombieSpawnSubtleClip, targetSpot.position, zombieSpawnVolume);

        // FIX: Borttaget det repeterande zombieGrowlClip-anropet härifrån så det inte spelas på varje klon!

        GameObject zombie = Instantiate(zombiePrefab, targetSpot.position, targetSpot.rotation);

        // 3. Lägg på det loopande rosslet på klonen
        if (zombieDefaultRosselClip != null)
        {
            AudioSource rosselSource = zombie.AddComponent<AudioSource>();
            rosselSource.clip = zombieDefaultRosselClip;
            rosselSource.volume = zombieSpawnVolume * 0.5f;
            rosselSource.spatialBlend = 1.0f;
            rosselSource.minDistance = 1f;
            rosselSource.maxDistance = 10f;
            rosselSource.loop = true;
            rosselSource.Play();
        }

        zombiesAlive++;
    }

    void GivePlayerHealthBoost(int amount)
    {
        if (playerAmateur != null)
        {
            var healthScript = playerAmateur.GetComponent<PlayerHealth>();
            if (healthScript != null)
            {
                Debug.Log("Hälsa boostad med +50 enheter!");
            }
        }
    }

    IEnumerator TurnOffPauseTextDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentWave != 6) SetAllPauseTextsActive(false);
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
            if (forcedIndex >= 0 && forcedIndex < zombieSpawnPoints.Length) indexToUse = forcedIndex;

            Transform selectedPoint = zombieSpawnPoints[indexToUse];
            if (selectedPoint != null)
            {
                SpawnZombieAtSpecificTransform(selectedPoint);
            }
        }
        return null;
    }

    public void EnemyDied()
    {
        zombiesAlive--;
        if (zombiesAlive <= 0 && !isSpawning)
        {
            if (currentWave == 6) return;

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