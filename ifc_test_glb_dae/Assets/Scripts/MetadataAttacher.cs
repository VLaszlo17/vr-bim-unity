using UnityEngine;

// A gyerek objektumokhoz metadata megjelen�t� komponens hozz�ad�sa.
public class MetadataAttacher : MonoBehaviour
{
    // J�t�k indul�sakor lefut
    void Start()
    {
        // V�gigiter�lunk az �sszes gyermek Transformon
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            // Ellen�rizz�k, hogy a MetadataLoader tartalmaz-e adatot az adott objektumhoz
            if (MetadataLoader.metadataDict != null &&
                MetadataLoader.metadataDict.ContainsKey(child.name))
            {
                // Ha m�g nincs MetadataDisplay komponens, hozz�adjuk
                if (!child.gameObject.TryGetComponent<MetadataDisplay>(out _))
                {
                    child.gameObject.AddComponent<MetadataDisplay>();
                }
            }
        }
    }
}
