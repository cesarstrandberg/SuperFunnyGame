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
        if (waveCard) waveCard.SetActive(false);
        if (deathCard) deathCard.SetActive(false);
    }

    public void ShowWaveComplete(int waveNumber)
    {
        waveStatusText.text = "WAVE " + waveNumber + " COMPLETED";
        StopAllCoroutines();
        StartCoroutine(WaveCardRoutine());
    }

    IEnumerator WaveCardRoutine()
    {
        waveCard.SetActive(true);
        yield return new WaitForSeconds(4f);
        waveCard.SetActive(false);
    }

    public void ShowGameOver(int waves)
    {
        deathCard.SetActive(true);
        finalWavesText.text = "TOTAL WAVES CLEARED: " + waves;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Fryser spelet så man kan titta på sitt visitkort
        Time.timeScale = 0f;
    }

    // Denna funktion kopplar du till din RETRY-knapp i Unity
    public void PlayAgain()
    {
        Time.timeScale = 1f; // Viktigt: Starta tiden igen!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}