using UnityEngine;

public class EscapeDoor : MonoBehaviour
{
    private void Start()
    {
        if (GetComponent<Collider>() == null || !GetComponent<Collider>().isTrigger)
        {
            Debug.LogError(gameObject.name + " måste ha en Box Collider där 'Is Trigger' är ikryssad!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Vi är på Våg 6 (Finalen) och spelaren kliver in i dörren!
            if (WaveManager.instance != null && WaveManager.instance.currentWave == 6)
            {
                Debug.Log("SPELAREN DETEKTERAD VID YTTERDÖRREN. ANROPAR VINST!");
                if (GameUIHandler.instance != null)
                {
                    GameUIHandler.instance.ShowVictory();
                }
            }
        }
    }
}