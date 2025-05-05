using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

// Unity Editor ablak fájlok letöltéséhez egy API szerverrõl (.glb és .json).
public class DownloadFromAPIEditor : EditorWindow
{
    private string apiBaseUrl = "http://127.0.0.1:8000"; // API szerver alap URL-je
    private string glbFileName = "column.glb"; // Letöltendõ GLB fájl neve
    private string jsonFileName = "column.json"; // Letöltendõ JSON fájl neve
    private string savePath = "Assets/DownloadedModels"; // Mentési hely Unity projektben

    // Menü gomb az Editorban az ablak megnyitására
    [MenuItem("Tools/Letöltés API-ból (.glb + .json)")]
    public static void ShowWindow()
    {
        GetWindow<DownloadFromAPIEditor>("Letöltés API-ból");
    }

    // Editor ablak GUI felület megrajzolása
    private void OnGUI()
    {
        GUILayout.Label("API Beállítások", EditorStyles.boldLabel);
        apiBaseUrl = EditorGUILayout.TextField("API base URL", apiBaseUrl);
        glbFileName = EditorGUILayout.TextField("GLB fájl neve", glbFileName);
        jsonFileName = EditorGUILayout.TextField("JSON fájl neve", jsonFileName);

        // Letöltés gomb megnyomásakor indítjuk a letöltést
        if (GUILayout.Button("Letöltés és importálás"))
        {
            DownloadFile(glbFileName);
            DownloadFile(jsonFileName);
            AssetDatabase.Refresh(); // Assetek frissítése Unity-ben
            Debug.Log("Letöltés és importálás kész.");
        }
    }

    // Egy adott fájl letöltése az API szerverrõl
    private void DownloadFile(string fileName)
    {
        string url = $"{apiBaseUrl}/download/{fileName}";
        string fullPath = Path.Combine(savePath, fileName);

        // Ha már létezik a fájl, rákérdezünk, hogy felülírjuk-e
        if (File.Exists(fullPath))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Fájl már létezik",
                $"{fileName} már létezik a mappában.\nFelül szeretnéd írni?",
                "Igen, felülírom",
                "Nem, kihagyom");

            if (!overwrite)
            {
                Debug.Log($"Letöltés kihagyva: {fileName} már létezik.");
                return;
            }
        }

        try
        {
            // HTTP GET kérés küldése a fájl elérésére
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var fileStream = File.Create(fullPath))
                    {
                        stream.CopyTo(fileStream); // Letöltött tartalom mentése
                        Debug.Log($"Letöltve: {fileName}");
                    }
                }
            }
        }
        catch (WebException ex)
        {
            // Ha a fájl nem található vagy más hiba történik
            if (ex.Response is HttpWebResponse errorResponse && errorResponse.StatusCode == HttpStatusCode.NotFound)
            {
                Debug.LogWarning($"Nincs ilyen feltöltött fájl: {fileName}");
            }
            else
            {
                Debug.LogError($"Hiba történt a letöltés során: {fileName} - {ex.Message}");
            }
        }
    }
}
