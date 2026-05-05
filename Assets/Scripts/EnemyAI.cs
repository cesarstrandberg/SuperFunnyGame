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
    public float attackDistance = 1.2f;
    public float attackRate = 1.5f;
    public float enemyDamage = 15f;

    private float nextAttackTime = 0f;
    private bool isDead = false;
    private bool hasAgonized = false;

    void Update()
    {
        if (isDead) return;

        // Kolla om vi är upptagna med att ha ont
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("ReactionHit") || state.IsName("Agonizing"))
        {
            agent.isStopped = true;
            return;
        }

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
        // Vänta 0.4 sekunder så näven hinner fram innan skadan sker
        Invoke("DealDamage", 0.4f);
    }

    void DealDamage()
    {
        if (isDead) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackDistance + 0.5f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(enemyDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        Debug.Log("Zombien tog skada! HP: " + health);

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
        if (isDead) return;
        isDead = true;
        agent.isStopped = true;
        anim.SetTrigger("Die");
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
}