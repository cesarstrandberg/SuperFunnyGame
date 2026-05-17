using UnityEngine;

public class EscapeDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Kolla om det är spelaren som kliver in i dörren
        if (other.CompareTag("Player"))
        {
            // Dörren är BARA aktiv om vi har nått sista vågen (Våg 5)
            if (WaveManager.instance != null && WaveManager.instance.currentWave == 66)
            {
                Debug.Log("SPELAREN DETEKTERAD VID DÖRREN! VINST!");
                if (GameUIHandler.instance != null)
                {
                    GameUIHandler.instance.ShowVictory();
                }
            }
        }
    }
}