using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar;

    [Header("Death Settings")]
    public bool isDead = false;
    public Image bloodyScreenOverlay;

    [Header("Starter Assets")]
    public MonoBehaviour thirdPersonController;
    public MonoBehaviour starterAssetsInputs;

    [Header("Audio")]
    public AudioSource voiceSource;
    public AudioClip deathQuoteLoop;
    public AudioClip hitQuote;
    [Range(0, 1)] public float hitQuoteChance = 0.2f;

    void Start()
    {
        currentHealth = maxHealth;
        if (bloodyScreenOverlay) bloodyScreenOverlay.gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (healthBar != null) healthBar.fillAmount = currentHealth / maxHealth;

        // Slumpmässig chans för "Not the face"
        if (voiceSource != null && hitQuote != null && !voiceSource.isPlaying)
        {
            if (Random.value < hitQuoteChance) voiceSource.PlayOneShot(hitQuote);
        }

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (bloodyScreenOverlay) bloodyScreenOverlay.gameObject.SetActive(true);

        if (thirdPersonController != null) thirdPersonController.enabled = false;
        if (starterAssetsInputs != null) starterAssetsInputs.enabled = false;

        if (voiceSource != null && deathQuoteLoop != null)
        {
            voiceSource.Stop();
            voiceSource.ignoreListenerPause = true;
            voiceSource.clip = deathQuoteLoop;
            voiceSource.loop = true;
            voiceSource.Play();
        }

        if (GameUIHandler.instance != null)
        {
            int score = WaveManager.instance.currentWave - 1;
            GameUIHandler.instance.ShowGameOver(score);
        }
    }
}