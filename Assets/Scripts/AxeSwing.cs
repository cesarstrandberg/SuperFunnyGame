using UnityEngine;
using UnityEngine.InputSystem;

public class AxeSwing : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;

    [Header("Settings")]
    public float attackRange = 2.5f;
    public float attackCooldown = 1.0f;
    public int damageAmount = 25; // Hur mycket skada yxan gör
    private float nextAttackTime = 0f;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            Swing();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Swing()
    {
        if (animator != null)
        {
            // Reset gör att vi kan slå igen direkt utan att "fastna"
            animator.ResetTrigger("Swing");
            animator.SetTrigger("Swing");
        }

        // 1. Hitta kamerans riktning
        Transform cam = cameraTransform != null ? cameraTransform : Camera.main.transform;
        Vector3 direction = cam.forward;

        // 2. Starta strålen från SPELARENS bröst (inte kamerans position)
        // transform.position är PlayerArmature. Vi lägger till 1.5m för att komma upp till bröstet.
        Vector3 origin = transform.position + Vector3.up * 1.5f;

        RaycastHit hit;

        // RITA LASERN - Nu bör den skjuta rakt framåt dit du tittar!
        Debug.DrawRay(origin, direction * attackRange, Color.red, 1.0f);

        if (Physics.Raycast(origin, direction, out hit, attackRange))
        {
            Debug.Log("Träffade: " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageAmount);
                }
            }
        }
    }
}