using UnityEngine;
using TMPro; // Denna rad krävs för att kunna styra TextMeshPro-texter

public class KillCounter : MonoBehaviour
{
    // Detta gör att andra skript kan hitta KillCounter enkelt
    public static KillCounter instance;

    public int count = 0;
    public TextMeshProUGUI killText; // Dra in din UI-text här i Unity

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddKill()
    {
        count++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (killText != null)
        {
            // Här är din episka text!
            killText.text = "Aaul Pallen's killed: " + count;
        }
    }
}