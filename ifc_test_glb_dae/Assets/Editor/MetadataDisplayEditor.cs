using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

// MetadataDisplay egyedi Unity Editor megjelenítéséért felelős osztály.
[CustomEditor(typeof(MetadataDisplay))]
public class MetadataDisplayEditor : Editor
{
    // Inspector ablak tartalmának megjelenítése
    public override void OnInspectorGUI()
    {
        // Lekérjük az aktuális MetadataDisplay komponenst
        MetadataDisplay display = (MetadataDisplay)target;

        // A GameObject nevét használjuk az ID azonosításához
        string id = display.gameObject.name;

        // Ellenőrizzük, hogy van-e metaadat az adott ID-re
        if (MetadataLoader.metadataDict != null && MetadataLoader.metadataDict.TryGetValue(id, out var metadata))
        {
            // Alapadatok megjelenítése
            EditorGUILayout.LabelField("Alapadatok", EditorStyles.boldLabel);
            DrawField("GlobalId", metadata.GlobalId);
            DrawField("Class", metadata.Class);
            DrawField("Name", metadata.Name);
            DrawField("Level", metadata.Level);
            DrawField("Type", metadata.PredefinedType);
            DrawField("ObjectType", metadata.ObjectType);

            EditorGUILayout.Space(5);

            // QuantitySets megjelenítése
            if (metadata.QuantitySets != null && metadata.QuantitySets.Count > 0)
            {
                EditorGUILayout.LabelField("QuantitySets", EditorStyles.boldLabel);
                foreach (var set in metadata.QuantitySets)
                {
                    EditorGUILayout.LabelField($"{set.Key}", EditorStyles.miniBoldLabel);
                    if (set.Value is JObject qtoObj)
                    {
                        foreach (var prop in qtoObj)
                            DrawField(prop.Key, prop.Value?.ToString());
                    }
                    else
                    {
                        DrawField(set.Key, set.Value?.ToString());
                    }
                }
            }

            // PropertySets megjelenítése
            if (metadata.PropertySets != null && metadata.PropertySets.Count > 0)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("PropertySets", EditorStyles.boldLabel);
                foreach (var set in metadata.PropertySets)
                {
                    EditorGUILayout.LabelField($" {set.Key}", EditorStyles.miniBoldLabel);
                    if (set.Value is JObject props)
                    {
                        foreach (var prop in props)
                            DrawField(prop.Key, prop.Value?.ToString());
                    }
                    else
                    {
                        DrawField(set.Key, set.Value?.ToString());
                    }
                }
            }
        }
        else
        {
            // Ha nincs metaadat, tájékoztató üzenet
            EditorGUILayout.HelpBox("Nincs metaadat ehhez az objektumhoz.", MessageType.Info);
        }
    }

    // Egy mező (kulcs-érték pár) megjelenítése sorban
    void DrawField(string label, string value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(150));
        EditorGUILayout.LabelField(value ?? "-", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
    }
}
