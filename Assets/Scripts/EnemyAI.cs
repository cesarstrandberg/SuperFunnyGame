using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip deathSound;

    [Header("Stats")]
    public float health = 75f;
    public float attackDistance = 1.1f;
    public float attackRate = 1.2f;
    public float enemyDamage = 15f;

    private bool isDead = false;
    private float nextAttackTime = 0f;
    private PlayerHealth playerHealth; // NY: Referens till spelarens hälsa

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Hämta hälso-skriptet från spelaren
        if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (isDead) return;

        // NYTT: Om spelaren är död, sluta jaga och attackera!
        if (playerHealth != null && playerHealth.isDead)
        {
            agent.isStopped = true;
            agent.enabled = false; // Stäng av NavMeshAgent helt
            anim.SetFloat("Speed", 0); // Gå till Idle
            return; // Avbryt resten av logiken
        }

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("ReactionHit")) { agent.isStopped = true; return; }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackDistance)
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            if (Time.time >= nextAttackTime) { Attack(); nextAttackTime = Time.time + attackRate; }
        }
        else { MoveToPlayer(); }
    }

    void MoveToPlayer()
    {
        if (isDead || !agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        if (audioSource && attackSound) audioSource.PlayOneShot(attackSound);
        Invoke("DealDamage", 0.4f);
    }

    void DealDamage()
    {
        if (isDead) return;
        // Vi TakeDamage sköter kollen internt om vi kan ta skada
        if (playerHealth != null) playerHealth.TakeDamage(enemyDamage);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;
        if (health <= 0.1f) Die();
        else anim.SetTrigger("Hit");
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (WaveManager.instance != null) WaveManager.instance.EnemyDied();
        agent.isStopped = true;
        agent.enabled = false;
        anim.SetTrigger("Die");
        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
}