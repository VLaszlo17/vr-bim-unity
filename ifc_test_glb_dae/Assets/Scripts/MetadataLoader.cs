using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

// JSON fájlból metaadatok betöltését végző osztály
public class MetadataLoader : MonoBehaviour
{
    [Header("Válaszd ki a JSON fájlt Inspectorban (TextAsset)")]
    public TextAsset jsonFile;  // A metaadatokat tartalmazó JSON fájl (ha kézzel állítják be Inspectorban)

    public static Dictionary<string, Metadata> metadataDict; // Globális dictionary a betöltött metaadatokkal

    // Játék ébredésekor automatikusan lefut
    void Awake()
    {
        // Ha megadtunk JSON fájlt az Inspectorban, abból betöltjük az adatokat
        if (jsonFile != null)
        {
            LoadFromJsonString(jsonFile.text);
            Debug.Log("[MetadataLoader] Metaadatok betöltve az Inspectorban megadott JSON fájlból.");
        }
        else
        {
            // Ha nincs megadva, csak később várunk futás közbeni betöltésre
            Debug.Log("[MetadataLoader] Nem adtál meg JSON fájlt az Inspectorban, futás közben várjuk a betöltést.");
        }
    }

    // Metaadatok betöltése egy JSON szövegből
    public void LoadFromJsonString(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("[MetadataLoader] Üres JSON szöveget kaptunk!");
            return;
        }

        try
        {
            // JSON deszerializálása Metadata objektumok listájává
            List<Metadata> elements = JsonConvert.DeserializeObject<List<Metadata>>(jsonText);

            // Új szótár létrehozása
            metadataDict = new Dictionary<string, Metadata>();
            foreach (var element in elements)
            {
                // Csak akkor mentjük, ha az elemnek van érvényes GlobalId-ja
                if (!string.IsNullOrEmpty(element.GlobalId))
                {
                    metadataDict[element.GlobalId] = element;
                }
            }

            Debug.Log($"[MetadataLoader] Metaadatok betöltve stringből: {metadataDict.Count} elem");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[MetadataLoader] Hiba a JSON feldolgozásakor: {ex.Message}");
        }
    }
}
