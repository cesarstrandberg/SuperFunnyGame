using UnityEngine;
using UnityEngine.InputSystem; // Necessary for the New Input System

public class AxeSwing : MonoBehaviour
{
    [Header("References")]
    public Animator animator;      // Drag PlayerArmature here in the Inspector
    public Transform cameraTransform; // Drag Main Camera here in the Inspector

    [Header("Settings")]
    public float attackRange = 2.5f;   // How far the axe reaches
    public float attackCooldown = 1.0f; // Time between swings
    private float nextAttackTime = 0f;

    void Update()
    {
        // Check if Left Mouse is clicked (New Input System way) AND if enough time has passed
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            Swing();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Swing()
    {
        // 1. Tell the Animator to play the "Swing" animation
        if (animator != null)
        {
            animator.SetTrigger("Swing");
        }

        // 2. Physics: Create an invisible "laser" (Raycast) from the camera
        // If cameraTransform is not assigned, we use the Main Camera as a fallback
        Vector3 origin = cameraTransform != null ? cameraTransform.position : Camera.main.transform.position;
        Vector3 direction = cameraTransform != null ? cameraTransform.forward : Camera.main.transform.forward;

        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        // 3. Check if that laser hits something within range
        if (Physics.Raycast(ray, out hit, attackRange))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // This draws a red line in your Scene View (for debugging)
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1.0f);

            // OPTIONAL: If the object has an "Enemy" tag, do something!
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Killed a Suit-Zombie!");
                // hit.collider.gameObject.SetActive(false); // Temporary "kill" logic
            }
        }
    }
}