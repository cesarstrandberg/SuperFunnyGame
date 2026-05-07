using UnityEngine;
using TMPro;
using System.Collections;

public class BusinessCardUI : MonoBehaviour
{
    public static BusinessCardUI instance;

    [Header("UI Element")]
    public GameObject cardObject;      // Dra in ditt vita kort (Panelen) här
    public TextMeshProUGUI waveStatusText; // Dra in Wave-texten här

    [Header("Inställningar")]
    public float displayDuration = 4f; // Hur länge kortet syns (Sätt till 4 eller 5)
    public float moveSpeed = 3f;       // Hur snabbt texten svävar
    public float moveAmount = 4f;      // Hur mycket texten svävar

    void Awake() { instance = this; }

    void Start()
    {
        if (cardObject != null) cardObject.SetActive(false);
    }

    public void ShowWaveComplete(int waveNumber)
    {
        // Den här raden gör att din "Wave 0"-text uppdateras automatiskt
        waveStatusText.text = "WAVE " + waveNumber + " COMPLETED";
        StopAllCoroutines(); // Avbryt om en gammal animation körs
        StartCoroutine(DisplayRoutine());
    }

    IEnumerator DisplayRoutine()
    {
        cardObject.SetActive(true);
        float timer = displayDuration;

        Vector3 startPos = waveStatusText.transform.localPosition;

        while (timer > 0)
        {
            // Subtil animation för "Vice President"-raden
            float newY = startPos.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
            waveStatusText.transform.localPosition = new Vector3(startPos.x, newY, startPos.z);

            timer -= Time.deltaTime;
            yield return null;
        }

        cardObject.SetActive(false);
        waveStatusText.transform.localPosition = startPos;
    }
}