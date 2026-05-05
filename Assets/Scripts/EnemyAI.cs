using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;

    void Update()
    {
        // Säg till gubben att gå mot spelaren
        agent.SetDestination(player.position);

        // Berätta för Animatorn hur snabbt vi går (så den byter till Walk)
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }
}