using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public AudioSource audioSource;

    [Header("Ljud")]
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip killQuoteClip;

    [Header("Stats")]
    public float health = 75f; // 3 slag med 25 skada
    public float attackDistance = 1.1f;
    public float attackRate = 1.2f; // Lite snabbare för mer hardcore känsla
    public float enemyDamage = 15f; // Lite mer skada för att öka pressen

    private bool isDead = false;
    private float nextAttackTime = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (isDead) return;

        // Om de precis blivit träffade, stanna till kort, sen fortsätt jaga
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("ReactionHit"))
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
        else { MoveToPlayer(); }
    }

    void MoveToPlayer()
    {
        if (isDead || agent.enabled == false) return;
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
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null) ph.TakeDamage(enemyDamage);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        // Detta skriver ut i konsolen exakt hur mycket liv som är kvar
        Debug.Log("Zombien tog skada! HP kvar: " + health);

        if (health <= 0.1f)
        {
            Debug.Log("Zombien dog nu!");
            Die();
        }
        else
        {
            // Vi kör BARA Hit om zombien faktiskt lever
            anim.SetTrigger("Hit");
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Rapportera till spawner
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null) spawner.EnemyDied();

        agent.isStopped = true;
        agent.enabled = false;

        anim.SetTrigger("Die");

        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);

        // Bateman-citat
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null && ph.voiceSource != null && killQuoteClip != null)
        {
            ph.voiceSource.PlayOneShot(killQuoteClip);
        }

        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        if (KillCounter.instance != null) KillCounter.instance.AddKill();
    }
}