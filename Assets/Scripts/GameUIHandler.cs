using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler instance;

    [Header("Visitkort (Panels)")]
    public GameObject waveCard;
    public GameObject deathCard;

    [Header("Texter")]
    public TextMeshProUGUI waveStatusText;
    public TextMeshProUGUI finalWavesText;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f; // Säkerställ att tiden går när vi startar
    }

    public void ShowWaveComplete(int waveNumber)
    {
        if (waveStatusText != null) waveStatusText.text = "WAVE " + waveNumber + " COMPLETED";
        StopAllCoroutines();
        StartCoroutine(WaveCardRoutine());
    }

    IEnumerator WaveCardRoutine()
    {
        if (waveCard) waveCard.SetActive(true);
        yield return new WaitForSeconds(4f);
        if (waveCard) waveCard.SetActive(false);
    }

    public void ShowGameOver(int waves)
    {
        if (deathCard != null) deathCard.SetActive(true);
        if (finalWavesText != null) finalWavesText.text = "TOTAL WAVES CLEARED: " + waves;

        // VIKTIGT: Släpp musen fri
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f; // Frys spelet
    }

    public void PlayAgain()
    {
        Debug.Log("Retry tryckt! Startar om...");
        Time.timeScale = 1f; // MÅSTE återställas innan load
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}