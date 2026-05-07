using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referenser")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public AudioSource audioSource;

    [Header("Zombie Sounds")]
    public AudioClip attackSound;
    public AudioClip agonySound;
    public AudioClip deathSound;

    [Header("Bateman Kill Quote")]
    public AudioClip killQuoteClip;
    public float quoteDelay = 1.2f;

    [Header("Inställningar")]
    public float health = 75f; // 3 slag om yxan gör 25
    public float walkSpeed = 3.5f;
    public float runSpeed = 8.0f;
    public float runDistance = 2.5f;
    public float attackDistance = 1.1f;
    public float attackRate = 1.5f;
    public float enemyDamage = 10f;

    private bool isDead = false;
    private bool hasAgonized = false;
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

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        // Om zombien reagerar på träff eller skriker, stå still
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
        if (isDead) return;

        agent.isStopped = false;
        agent.speed = (distance > runDistance) ? runSpeed : walkSpeed;
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Attack()
    {
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Attack");
        if (audioSource && attackSound) audioSource.PlayOneShot(attackSound);
        Invoke("DealDamage", 0.4f);
    }

    void DealDamage()
    {
        if (isDead) return;
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackDistance + 0.8f)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(enemyDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        // Vi resettar triggers för att vara säkra på att rätt animation prioriteras
        anim.ResetTrigger("Hit");
        anim.SetTrigger("Hit");

        // AGONY: Sker efter slag 2 (75 - 25 - 25 = 25 HP kvar)
        // Vi kollar om HP är mellan 1 och 26
        if (health > 0 && health <= 26 && !hasAgonized)
        {
            hasAgonized = true;
            anim.SetTrigger("Agony");
            if (agonySound && audioSource) audioSource.PlayOneShot(agonySound);
        }

        // DIE: Sker när HP når 0 eller mindre
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Stäng av AI helt så den inte flyttar liket
        agent.isStopped = true;
        agent.enabled = false;

        // 2. Kör animationen
        anim.ResetTrigger("Hit");
        anim.ResetTrigger("Agony");
        anim.SetTrigger("Die");

        // 3. Ljud
        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);
        Invoke("PlayBatemanKillQuote", quoteDelay);

        // 4. Stäng av kollision så spelaren kan gå genom liket
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;

        // 5. Uppdatera räknaren
        if (KillCounter.instance != null) KillCounter.instance.AddKill();

        // Vi kör INTE Destroy(gameObject) för vi vill ha kvar liket!
    }

    void PlayBatemanKillQuote()
    {
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null && ph.voiceSource != null && killQuoteClip != null)
        {
            ph.voiceSource.PlayOneShot(killQuoteClip);
        }
    }
}