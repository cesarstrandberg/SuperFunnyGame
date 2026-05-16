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

    [Header("Sami's Sight System")]
    public float sightRange = 12f;      // Hur långt bort de kan se dig
    public bool hasDetectedPlayer = false; // Börjar som false (de sover/väntar)

    private bool isDead = false;
    private float nextAttackTime = 0f;
    private PlayerHealth playerHealth;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (isDead || (playerHealth != null && playerHealth.isDead))
        {
            if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // --- NYTT: DETEKTIONS-LOGIK ---
        if (!hasDetectedPlayer)
        {
            // Om spelaren kommer inom synhåll, eller om vi blir träffade (sker i TakeDamage)
            if (distance <= sightRange)
            {
                hasDetectedPlayer = true;
            }
            else
            {
                // Om vi inte sett spelaren än: Stå still och gör inget
                if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
                anim.SetFloat("Speed", 0);
                return;
            }
        }
        // ------------------------------

        // Härifrån och neråt är din ORIGINAL-LOGIK helt orörd!
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
        if (playerHealth != null) playerHealth.TakeDamage(enemyDamage);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // Pro-tip: Om Patrick hugger dem, fattar de direkt var han är!
        hasDetectedPlayer = true;

        health -= damage;
        if (health <= 0.1f) Die();
        else anim.SetTrigger("Hit");
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (KillCounter.instance != null) KillCounter.instance.AddKill();
        if (WaveManager.instance != null) WaveManager.instance.EnemyDied();

        agent.isStopped = true;
        agent.enabled = false;
        anim.SetTrigger("Die");
        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
}