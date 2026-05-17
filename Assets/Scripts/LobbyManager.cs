using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public GameObject aboutPanel;

    [Header("Ljudinställningar")]
    public AudioSource lobbyAudioSource;
    public AudioClip readyUpClickClip;
    public AudioClip aboutClickClip;     // NY: Slot för About-knappens klickljud!

    private bool isStarting = false;

    void Start()
    {
        // Detta tvingar fram muspekaren och låser upp den från mitten av skärmen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        if (isStarting) return;
        StartCoroutine(StartGameRoutine());
    }

    IEnumerator StartGameRoutine()
    {
        isStarting = true;

        if (lobbyAudioSource != null && readyUpClickClip != null)
        {
            lobbyAudioSource.PlayOneShot(readyUpClickClip);
        }

        yield return new WaitForSeconds(0.8f);

        SceneManager.LoadScene("PatrickBatemanApartment");
        Time.timeScale = 1f;
    }

    // UPPDATERAD: Fyrar av det nya klickljudet i samma millisekund som panelen öppnas!
    public void OpenAbout()
    {
        if (lobbyAudioSource != null && aboutClickClip != null)
        {
            lobbyAudioSource.PlayOneShot(aboutClickClip);
        }

        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true);
        }
    }

    public void CloseAbout()
    {
        // TIPS: Om du vill ha ett ljud även när man stänger About-panelen, 
        // kan du bara klistra in samma PlayOneShot-rader här under!
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
        }
    }
}