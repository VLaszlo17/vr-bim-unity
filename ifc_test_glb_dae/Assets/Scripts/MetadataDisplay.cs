using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// Egy elemhez tartozó metaadatok megjelenítéséért felelős osztály UI-n.
public class MetadataDisplay : MonoBehaviour
{
    [TextArea(5, 20)]
    public string metadataText; // A metaadatok szöveges megjelenítése

    public Metadata metadata; // A metaadatok tárolására szolgáló objektum

    // Inspectorban változáskor automatikus frissítés
    void OnValidate()
    {
        UpdateMetadata();
    }

    // Játék indulásakor lefut
    void Start()
    {
        UpdateMetadata();
    }

    // Metaadat frissítése a MetadataLoader alapján
    public void UpdateMetadata()
    {
        // Ha nincs betöltve a metadata dictionary, kilépünk
        if (MetadataLoader.metadataDict == null)
            return;

        // Megpróbáljuk betölteni a metaadatot az adott GameObject nevéhez
        if (MetadataLoader.metadataDict.TryGetValue(gameObject.name, out var meta))
        {
            metadata = meta;
            metadataText = GetFormattedMetadata();
        }
        else
        {
            // Ha nincs metaadat, kiírjuk
            metadataText = "Nincs metaadat ehhez az elemhez.";
        }
    }

    // A metaadatok szépen formázott szöveggé alakítása
    public string GetFormattedMetadata()
    {
        // Ha nincs metaadat, rögtön visszatérünk
        if (metadata == null)
            return "Nincs metaadat.";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // Alapadatok szekció
        sb.AppendLine("<size=40><b><color=#FFFFFF>Alapadatok</color></b></size>");
        sb.AppendLine($"<b>GlobalId:</b> <color=#f5f2f2>{metadata.GlobalId}</color>");
        sb.AppendLine($"<b>Class:</b> <color=#f5f2f2>{metadata.Class}</color>");
        sb.AppendLine($"<b>Name:</b> <color=#f5f2f2>{metadata.Name}</color>");
        sb.AppendLine($"<b>Level:</b> <color=#f5f2f2>{metadata.Level}</color>");
        sb.AppendLine($"<b>ObjectType:</b> <color=#f5f2f2>{metadata.ObjectType}</color>");
        sb.AppendLine($"<b>Type:</b> <color=#f5f2f2>{metadata.PredefinedType}</color>");
        sb.AppendLine("");

        // QuantitySets szekció
        if (metadata.QuantitySets != null && metadata.QuantitySets.Count > 0)
        {
            sb.AppendLine("<size=32><b><color=#1c41fc>QuantitySets</color></b></size>");
            foreach (var set in metadata.QuantitySets)
            {
                sb.AppendLine($"<b><color=#1c41fc>{set.Key}</color></b>");
                if (set.Value is JObject obj)
                {
                    foreach (var prop in obj)
                    {
                        sb.AppendLine($"<color=#51bef0>{prop.Key}:</color> {prop.Value}");
                    }
                }
            }
            sb.AppendLine("");
        }

        // PropertySets szekció
        if (metadata.PropertySets != null && metadata.PropertySets.Count > 0)
        {
            sb.AppendLine("<size=32><b><color=#00750e>PropertySets</color></b></size>");
            foreach (var set in metadata.PropertySets)
            {
                sb.AppendLine($"<b><color=#00750e>{set.Key}</color></b>");
                if (set.Value is JObject obj)
                {
                    foreach (var prop in obj)
                    {
                        sb.AppendLine($"<color=#50ed0c>{prop.Key}:</color> {prop.Value}");
                    }
                }
            }
        }

        return sb.ToString();
    }
}
