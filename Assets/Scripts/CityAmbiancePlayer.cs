using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class CityAmbiancePlayer : MonoBehaviour
{
    [Header("Dina 2 ljudfiler för detta fönster")]
    public AudioClip[] cityClips = new AudioClip[2]; // Låst till 2 slots i Inspectorn!

    [Header("Hur ofta ska det låta? (Sekunder)")]
    [Tooltip("Minsta tid att vänta innan ett nytt ljud spelas")]
    public float minTimeBetweenSounds = 40f;

    [Tooltip("Maximal tid att vänta innan ett nytt ljud spelas")]
    public float maxTimeBetweenSounds = 80f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Tvingar AudioSourcen till 3D-ljud och stänger av PlayOnAwake via kod
        audioSource.spatialBlend = 1.0f;
        audioSource.playOnAwake = false;

        // Starta klockan som slumpar ljuden
        if (cityClips != null && cityClips.Length > 0)
        {
            StartCoroutine(PlayRandomSoundRoutine());
        }
    }

    IEnumerator PlayRandomSoundRoutine()
    {
        // Körs i bakgrunden hela spelet
        while (true)
        {
            // 1. Slumpa en tid att vänta
            float waitTime = Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
            yield return new WaitForSeconds(waitTime);

            // 2. Välj ett av de två ljuden helt slumpmässigt
            int randomIndex = Random.Range(0, cityClips.Length);
            AudioClip chosenClip = cityClips[randomIndex];

            // 3. Spela upp ljudet som en engångseffekt (one-shot)
            if (chosenClip != null)
            {
                audioSource.PlayOneShot(chosenClip);
                Debug.Log(gameObject.name + " spelade stads-ljud: " + chosenClip.name);
            }
        }
    }
}