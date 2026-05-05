using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referenser")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    [Header("Inställningar")]
    public float health = 100f;
    public float walkSpeed = 3.5f;
    public float runSpeed = 8.0f;
    public float runDistance = 2.5f;
    public float attackDistance = 1.1f;
    public float attackRate = 1.5f;

    private float nextAttackTime = 0f;
    private bool isDead = false;
    private bool hasAgonized = false;

    void Update()
    {
        if (isDead) return;

        // --- EXPERT-FIXEN: Stoppa all rörelse under smärta ---
        // Vi kollar om Animatorn just nu spelar "ReactionHit" eller "Agonizing"
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        bool isBusy = state.IsName("ReactionHit") || state.IsName("Agonizing");

        if (isBusy)
        {
            agent.isStopped = true; // Stanna fötterna
            anim.SetFloat("Speed", 0); // Säg till animatorn att vi står still
            return; // Hoppa över resten av Update (ingen jagande!)
        }
        // ---------------------------------------------------

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackRate;
            }
        }
        else
        {
            Move(distance);
        }

        if (Input.GetKeyDown(KeyCode.K)) TakeDamage(25);
    }

    void Move(float distance)
    {
        agent.isStopped = false;
        agent.speed = (distance > runDistance) ? runSpeed : walkSpeed;
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Attack()
    {
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Attack");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;

        anim.SetTrigger("Hit");

        if (health <= 25 && health > 0 && !hasAgonized)
        {
            hasAgonized = true;
            anim.SetTrigger("Agony");
        }

        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        anim.SetTrigger("Die");
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
}