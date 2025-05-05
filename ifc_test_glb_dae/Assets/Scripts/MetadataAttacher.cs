using UnityEngine;

// A gyerek objektumokhoz metadata megjelenítõ komponens hozzáadása.
public class MetadataAttacher : MonoBehaviour
{
    // Játék indulásakor lefut
    void Start()
    {
        // Végigiterálunk az összes gyermek Transformon
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            // Ellenõrizzük, hogy a MetadataLoader tartalmaz-e adatot az adott objektumhoz
            if (MetadataLoader.metadataDict != null &&
                MetadataLoader.metadataDict.ContainsKey(child.name))
            {
                // Ha még nincs MetadataDisplay komponens, hozzáadjuk
                if (!child.gameObject.TryGetComponent<MetadataDisplay>(out _))
                {
                    child.gameObject.AddComponent<MetadataDisplay>();
                }
            }
        }
    }
}
