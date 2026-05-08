using UnityEngine;
using System.Collections;
using TMPro;

public class GameUIHandler : MonoBehaviour
{
    public static GameUIHandler instance;

    [Header("UI Paneler")]
    public GameObject waveCard;
    public GameObject deathCard;

    [Header("UI Texter")]
    public TextMeshProUGUI waveCompletedText;

    void Awake() { instance = this; }

    public void ShowWaveComplete(int waveNum)
    {
        if (waveCard != null)
        {
            if (waveCompletedText != null)
            {
                // HÄR FIXAR VI TEXTEN: Vi skriver hela meningen i koden
                // Detta gör att "COMPLETED" aldrig försvinner!
                waveCompletedText.text = "WAVE " + waveNum + " COMPLETED";
            }
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

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}