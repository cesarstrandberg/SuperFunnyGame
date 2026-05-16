using UnityEngine;

public class EnemyOutfits : MonoBehaviour
{
    [Header("Dina 5 kostym-materials (Simple Lit)")]
    public Material vanligBeigeMaterial;
    public Material svartMaterial;
    public Material blaMaterial;
    public Material morkBlaMaterial;
    public Material randigMaterial;

    [Header("Chans i procent (0 till 100)")]
    [Range(0, 100)] public float svartChans = 20f;
    [Range(0, 100)] public float blaChans = 20f;
    [Range(0, 100)] public float morkBlaChans = 15f;
    [Range(0, 100)] public float randigChans = 5f;

    void Start()
    {
        // AUTOMATISK LÖSNING: Hämta ALLA LOD-lager som finns på gubben samtidigt!
        SkinnedMeshRenderer[] allRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        if (allRenderers.Length == 0)
        {
            Debug.LogError("Hittade inga SkinnedMeshRenderers på " + gameObject.name);
            return;
        }

        // Slumpa ett tal mellan 0 och 100
        float slump = Random.Range(0f, 100f);
        float nuvarandeGrans = 0f;
        Material valtMaterial = vanligBeigeMaterial; // Standard om inget annat slår in

        // 1. Randig
        nuvarandeGrans += randigChans;
        if (slump < nuvarandeGrans)
        {
            valtMaterial = randigMaterial;
            Debug.Log(gameObject.name + " ska bli: RANDIG!");
        }
        // 2. Mörkblå
        else if (slump < (nuvarandeGrans + morkBlaChans))
        {
            valtMaterial = morkBlaMaterial;
            Debug.Log(gameObject.name + " ska bli: MÖRKBLÅ!");
        }
        // 3. Svart
        else if (slump < (nuvarandeGrans + morkBlaChans + svartChans))
        {
            valtMaterial = svartMaterial;
            Debug.Log(gameObject.name + " ska bli: SVART!");
        }
        // 4. Blå
        else if (slump < (nuvarandeGrans + morkBlaChans + svartChans + blaChans))
        {
            valtMaterial = blaMaterial;
            Debug.Log(gameObject.name + " ska bli: BLÅ!");
        }
        else
        {
            Debug.Log(gameObject.name + " ska bli: BEIGE (Standard)!");
        }

        // HÄR SKER MAGIN: Loopa igenom LOD0, LOD1, LOD3 och färga ALLA lagren!
        foreach (SkinnedMeshRenderer renderer in allRenderers)
        {
            if (valtMaterial == null) continue;

            // Hämta just den meshens material-slots
            Material[] lokalLista = renderer.materials;

            // Färgfyll alla slots som den meshen har (så vi slipper bry oss om Element 0 eller 1)
            for (int i = 0; i < lokalLista.Length; i++)
            {
                lokalLista[i] = valtMaterial;
            }

            // Skicka tillbaka till Unity
            renderer.materials = lokalLista;
        }
    }
}