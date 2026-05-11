using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public GameObject aboutPanel;

    void Start()
    {
        // Detta tvingar fram muspekaren och låser upp den från mitten av skärmen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        // Byt ut namnet mot ditt exakta scen-namn eller index 1
        SceneManager.LoadScene("PatrickBatemanApartment");
        Time.timeScale = 1f;
    }

    public void OpenAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true);
        }
    }

    // Denna funktion körs när man trycker på Exit-knappen inuti About-panelen
    public void CloseAbout()
    {
        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
        }
    }

}
