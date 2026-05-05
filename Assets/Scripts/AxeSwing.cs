using UnityEngine;
using UnityEngine.InputSystem;

public class AxeSwing : MonoBehaviour
{
    public Animator animator;
    public Transform cameraTransform;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.0f;
    public int damageAmount = 25;
    private float nextAttackTime = 0f;

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
        animator.SetTrigger("Swing");
        Transform cam = cameraTransform != null ? cameraTransform : Camera.main.transform;
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 direction = cam.forward;

        RaycastHit hit;
        Debug.DrawRay(origin, direction * attackRange, Color.red, 1.0f);

        if (Physics.Raycast(origin, direction, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null) enemy.TakeDamage(damageAmount);
            }
        }
    }
}