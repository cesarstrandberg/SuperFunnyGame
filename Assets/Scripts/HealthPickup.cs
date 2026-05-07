using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Inställningar")]
    public float healAmount = 30f;
    public AudioClip drinkSound;

    private void OnTriggerEnter(Collider other)
    {
        // Vi kollar om det är spelaren (Patrick) som går in i glaset
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();

            // Vi kollar mot ph.currentHealth eftersom det är vad det heter i ditt PlayerHealth-skript
            if (ph != null && ph.currentHealth < ph.maxHealth)
            {
                // 1. Ge hälsa (stoppa vid maxHealth)
                ph.currentHealth = Mathf.Min(ph.currentHealth + healAmount, ph.maxHealth);

                // 2. Uppdatera UI-mätaren så man ser att man får liv
                if (ph.healthBar != null)
                {
                    ph.healthBar.fillAmount = ph.currentHealth / ph.maxHealth;
                }

                // 3. Trigga animationen i spelarens Animator
                Animator playerAnim = other.GetComponentInChildren<Animator>();
                if (playerAnim != null)
                {
                    playerAnim.SetTrigger("Drink");
                }

                // 4. Spela ljudet av klirrande is/glas
                if (drinkSound && ph.voiceSource != null)
                {
                    ph.voiceSource.PlayOneShot(drinkSound);
                }

                Debug.Log("J&B Rare Scotch consumed. Health is now: " + ph.currentHealth);

                // 5. Ta bort hela glaset från bordet
                Destroy(gameObject);
            }
        }
    }
}