using UnityEngine;
using UnityEngine.InputSystem;

public class AxeSwing : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Header("Settings")]
    public float attackRange = 2.5f;
    public float attackCooldown = 1.0f;
    public int damageAmount = 25;
    private float nextAttackTime = 0f;

    [Header("Sounds")]
    public AudioClip swingSound;
    public AudioClip[] hitSounds;
    public AudioClip dorsiaQuote;
    [Range(0, 1)] public float quoteChance = 0.5f;

    [Header("Effects")]
    public GameObject bloodPrefab;

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
        if (animator != null) animator.SetTrigger("Swing");
        if (swingSound && sfxSource != null) sfxSource.PlayOneShot(swingSound);

        Transform cam = cameraTransform != null ? cameraTransform : Camera.main.transform;
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = cam.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (hitSounds.Length > 0 && sfxSource != null)
                    sfxSource.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);

                if (Random.value < quoteChance && dorsiaQuote != null && voiceSource != null)
                {
                    if (!voiceSource.isPlaying) voiceSource.PlayOneShot(dorsiaQuote);
                }
                if (bloodPrefab) Instantiate(bloodPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null) enemy.TakeDamage(damageAmount);
            }
        }
    }
}