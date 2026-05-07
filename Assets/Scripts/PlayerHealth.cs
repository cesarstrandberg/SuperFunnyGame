using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar;

    [Header("Death Settings")]
    public bool isDead = false;
    public Image bloodyScreenOverlay; // Dra in din röda Panel här

    [Header("Starter Assets (Dra in skripten här)")]
    // Dessa rader är nu aktiva (ingen // framför)
    public MonoBehaviour thirdPersonController;
    public MonoBehaviour starterAssetsInputs;

    [Header("Audio")]
    public AudioSource voiceSource;
    public AudioClip deathQuoteLoop;

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

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Visa den röda skärmen
        if (bloodyScreenOverlay) bloodyScreenOverlay.gameObject.SetActive(true);

        // 2. Inaktivera styrning (Detta fryser både gubben och kameran!)
        if (thirdPersonController != null) thirdPersonController.enabled = false;
        if (starterAssetsInputs != null) starterAssetsInputs.enabled = false;

        // 3. Spela det långa citatet på repeat
        if (voiceSource != null && deathQuoteLoop != null)
        {
            voiceSource.ignoreListenerPause = true;
            voiceSource.clip = deathQuoteLoop;
            voiceSource.loop = true;
            voiceSource.Play();
        }

        // 4. Visa Visitkortet
        if (GameUIHandler.instance != null)
        {
            int score = WaveManager.instance.currentWave - 1;
            GameUIHandler.instance.ShowGameOver(score);
        }
    }
}