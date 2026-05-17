using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler instance;

    [Header("UI Paneler")]
    public GameObject waveCard;
    public GameObject deathCard;
    public GameObject victoryCard; // NY: Skapa en panel för vinstskärmen och dra in den här!

    [Header("UI Texter")]
    public TextMeshProUGUI waveCompletedText;

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
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // NY FUNKTION: Visar vinstskärmen när man rör ytterdörren!
    public void ShowVictory()
    {
        if (victoryCard != null) victoryCard.SetActive(true);
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