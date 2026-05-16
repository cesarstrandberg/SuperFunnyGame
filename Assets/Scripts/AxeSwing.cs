using UnityEngine;
using UnityEngine.InputSystem;

public class AxeSwing : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;
    public Transform playerArmature;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Header("Settings")]
    public float attackRange = 2.5f;
    public float attackCooldown = 1.0f;
    public int damageAmount = 25;
    private float nextAttackTime = 0f;

    [Header("Sami's Impact Fix")]
    public LayerMask enemyLayer;
    public float sphereRadius = 0.3f;

    [Header("BloodFX from GitHub")]
    public GameObject bloodParticlePrefab;
    [Tooltip("Dra in prefaben för zombiestänket här")]
    public GameObject bloodDecalPrefab;
    [Tooltip("Dra in prefaben för golvpölen här")]
    public GameObject bloodPuddlePrefab;

    [Header("Layers")]
    public LayerMask playerLayer;

    [Header("Sounds")]
    public AudioClip swingSound;
    public AudioClip[] hitSounds;
    public AudioClip dorsiaQuote;
    [Range(0, 1)] public float quoteChance = 0.5f;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            Swing();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Swing()
    {
        Transform cam = cameraTransform != null ? cameraTransform : Camera.main.transform;

        // COMBAT SNAP
        Vector3 lookDir = cam.forward;
        lookDir.y = 0;
        if (playerArmature != null) playerArmature.rotation = Quaternion.LookRotation(lookDir);

        if (animator != null) animator.SetTrigger("Swing");
        if (swingSound && sfxSource != null) sfxSource.PlayOneShot(swingSound);

        Vector3 origin = cam.position + cam.forward * 0.5f;
        Vector3 direction = cam.forward;
        RaycastHit hit;

        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, attackRange, enemyLayer, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // ANIMATION RESET
                float dist = Vector3.Distance(origin, hit.point);
                if (dist < 1.3f)
                {
                    animator.Play("Idle Walk Run Blend", 0, 0f);
                }

                // LJUD & CITAT
                if (hitSounds.Length > 0 && sfxSource != null)
                    sfxSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);

                if (Random.value < quoteChance && dorsiaQuote != null && voiceSource != null)
                {
                    if (!voiceSource.isPlaying) voiceSource.PlayOneShot(dorsiaQuote);
                }

                // --- AVANCERAD BLOD-LOGIK ---
                Quaternion particleRotation = Quaternion.LookRotation(hit.normal);
                Quaternion decalRotation = Quaternion.LookRotation(-hit.normal);

                // A. Partikelsprutet i luften (Fastnar på zombien)
                if (bloodParticlePrefab != null)
                {
                    GameObject particles = Instantiate(bloodParticlePrefab, hit.point, particleRotation, hit.transform);
                    Destroy(particles, 4f);
                }

                // B. PÅ ZOMBIEN (Klistras fast på kroppen via hit.transform)
                if (bloodDecalPrefab != null)
                {
                    Instantiate(bloodDecalPrefab, hit.point, decalRotation, hit.transform);
                }

                // C. PÖL PÅ GOLVET
                if (bloodPuddlePrefab != null)
                {
                    RaycastHit groundHit;
                    Vector3 rayStart = hit.point + Vector3.up * 0.1f;

                    int ignoreCharactersMask = ~(enemyLayer | playerLayer);

                    if (Physics.Raycast(rayStart, Vector3.down, out groundHit, 5f, ignoreCharactersMask))
                    {
                        Quaternion floorRotation = Quaternion.LookRotation(-groundHit.normal);

                        // Tillbaka till 0.1f så att den svävar säkert över golvet
                        Vector3 puddleSpawnPos = groundHit.point + Vector3.up * 0.1f;

                        GameObject floorDecal = Instantiate(bloodPuddlePrefab, puddleSpawnPos, floorRotation);
                        Destroy(floorDecal, 25f);
                    }
                }
                // ----------------------------

                // SKADA
                EnemyAI enemy = hit.collider.GetComponentInParent<EnemyAI>();
                if (enemy != null) enemy.TakeDamage(damageAmount);
            }
        }
    }
}