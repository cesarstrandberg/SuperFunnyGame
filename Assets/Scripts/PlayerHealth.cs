using UnityEngine;
using UnityEngine.SceneManagement; // För att kunna starta om banan

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        // Notera STORT L i Log
        Debug.Log("AJ! Spelaren har " + currentHealth + " HP kvar.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Notera STORT L i Log
        Debug.Log("Du dog i Corporate Ladder... Game Over!");

        // Starta om banan
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}