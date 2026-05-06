using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Referenser")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public AudioSource audioSource; // Zombiens egen högtalare

    [Header("Zombie Sounds")]
    public AudioClip attackSound;
    public AudioClip agonySound;
    public AudioClip deathSound;

    [Header("Bateman Kill Quote")]
    public AudioClip killQuoteClip;
    public float quoteDelay = 1.2f;

    [Header("Inställningar")]
    public float health = 75f;
    public float walkSpeed = 3.5f;
    public float runSpeed = 8.0f;
    public float runDistance = 2.5f;
    public float attackDistance = 1.1f;
    public float attackRate = 1.5f;
    public float enemyDamage = 15f;

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
        else { Move(distance); }
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
        anim.SetTrigger("Hit");
        if (health <= 26 && health > 0 && !hasAgonized)
        {
            hasAgonized = true;
            anim.SetTrigger("Agony");
            if (agonySound && audioSource) audioSource.PlayOneShot(agonySound);
        }
        if (health <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        agent.isStopped = true;
        anim.SetTrigger("Die");
        if (deathSound && audioSource) audioSource.PlayOneShot(deathSound);
        Invoke("PlayBatemanKillQuote", quoteDelay);
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }

    void PlayBatemanKillQuote()
    {
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        // FIX: Här stod det audioSource förut, nu står det voiceSource!
        if (ph != null && ph.voiceSource != null && killQuoteClip != null)
        {
            ph.voiceSource.PlayOneShot(killQuoteClip);
        }
    }
}