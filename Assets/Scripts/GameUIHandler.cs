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
    public GameObject victoryCard; // Skapa din vinstpanel under Canvas och dra in den här!

    [Header("UI Texter")]
    public TextMeshProUGUI waveCompletedText;

    [Header("Ljud (Rundor)")]
    public AudioSource playerVoiceSource;
    public AudioClip waveCompleteQuote;

    [Header("NYTT: Ljud (Vinstskärm)")]
    public AudioSource victoryAudioSource; // Dra in din spelares röstkälla eller en separat AudioSource här
    public AudioClip victorySFXClip;      // Ditt check-ljud/pling när man vinner!
    public AudioClip victoryQuoteClip;    // Det sista episka Bateman-vinstcitatet!

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

    // UPPDATERAD: Fyrar nu av vinstljuden och citatet i samma millisekund som man klarar spelet!
    public void ShowVictory()
    {
        if (victoryCard != null) victoryCard.SetActive(true);

        // Spela upp vinstljuden direkt på din AudioSource
        if (victoryAudioSource != null)
        {
            if (victorySFXClip != null) victoryAudioSource.PlayOneShot(victorySFXClip);
            if (victoryQuoteClip != null) victoryAudioSource.PlayOneShot(victoryQuoteClip);
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