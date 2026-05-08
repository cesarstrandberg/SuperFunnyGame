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
        // 1. Kolla om fienden är död ELLER om spelaren är död
        if (isDead || (playerHealth != null && playerHealth.isDead))
        {
            // Stoppa bara om vi faktiskt är på banan (NavMesh)
            if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;

            // Se till att zombien slutar spela sin spring-animation
            anim.SetFloat("Speed", 0);
            return;
        }

        // 2. Beräkna avstånd till Patrick
        float distance = Vector3.Distance(transform.position, player.position);

        // 3. Attack-logik
        if (distance <= attackDistance)
        {
            if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
            anim.SetFloat("Speed", 0);

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackRate;
            }
        }
        // 4. Jaga-logik
        else
        {
            MoveToPlayer();
        }
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

        // Denna rad säger till räknaren att lägga till ett poäng:
        if (KillCounter.instance != null)
        {
            KillCounter.instance.AddKill();
        }

        if (WaveManager.instance != null) WaveManager.instance.EnemyDied();
        agent.isStopped = true;
        agent.enabled = false;
        anim.SetTrigger("Die");
        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
}