using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler instance;

    [Header("UI Paneler")]
    public GameObject waveCard;
    public GameObject deathCard;

    [Header("UI Texter")]
    public TextMeshProUGUI waveCompletedText;
    public TextMeshProUGUI finalScoreDisplayText; // Kom ihåg att dra in textrutan på ditt DeathCard här!

    [Header("Ljud")]
    public AudioSource playerVoiceSource;
    public AudioClip waveCompleteQuote;

    void Awake() { instance = this; }

    public void ShowWaveComplete(int waveNum)
    {
        if (waveCard != null)
        {
            if (waveCompletedText != null)
            {
                waveCompletedText.text = "WAVE " + (waveNum - 1) + " COMPLETED";
            }

            if (playerVoiceSource != null && waveCompleteQuote != null)
                playerVoiceSource.PlayOneShot(waveCompleteQuote);

            StopAllCoroutines();
            StartCoroutine(WaveCardRoutine());
        }
    }

    IEnumerator WaveCardRoutine()
    {
        waveCard.SetActive(true);
        yield return new WaitForSeconds(3f);
        waveCard.SetActive(false);
    }

    public void ShowGameOver(int finalScore)
    {
        if (deathCard != null) deathCard.SetActive(true);

        // FIX: Hämtar nu datan direkt från WaveManager så det blir stenhårt rätt!
        if (finalScoreDisplayText != null)
        {
            int wavesCleared = finalScore - 1;

            if (WaveManager.instance != null)
            {
                wavesCleared = WaveManager.instance.currentWave - 1;
            }

            if (wavesCleared < 0) wavesCleared = 0; // Säkerhetsbälte

            finalScoreDisplayText.text = "WAVES SURVIVED: " + wavesCleared;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
}