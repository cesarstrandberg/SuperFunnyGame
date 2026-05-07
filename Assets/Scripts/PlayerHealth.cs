using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource voiceSource;
    public AudioClip[] hurtSounds;
    public AudioClip[] hurtQuotes;
    [Range(0, 1)] public float quoteChance = 0.5f;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.fillAmount = 1f;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar != null) healthBar.fillAmount = currentHealth / maxHealth;

        if (hurtSounds.Length > 0 && sfxSource != null)
            sfxSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);

        if (Random.value < quoteChance && hurtQuotes.Length > 0 && voiceSource != null)
        {
            if (!voiceSource.isPlaying)
                voiceSource.PlayOneShot(hurtQuotes[Random.Range(0, hurtQuotes.Length)]);
        }

        if (currentHealth <= 0) Die();
    }

    void Die() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
}