using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

// Unity Editor ablak f�jlok let�lt�s�hez egy API szerverr�l (.glb �s .json).
public class DownloadFromAPIEditor : EditorWindow
{
    private string apiBaseUrl = "http://127.0.0.1:8000"; // API szerver alap URL-je
    private string glbFileName = "column.glb"; // Let�ltend� GLB f�jl neve
    private string jsonFileName = "column.json"; // Let�ltend� JSON f�jl neve
    private string savePath = "Assets/DownloadedModels"; // Ment�si hely Unity projektben

    // Men� gomb az Editorban az ablak megnyit�s�ra
    [MenuItem("Tools/Let�lt�s API-b�l (.glb + .json)")]
    public static void ShowWindow()
    {
        GetWindow<DownloadFromAPIEditor>("Let�lt�s API-b�l");
    }

    // Editor ablak GUI fel�let megrajzol�sa
    private void OnGUI()
    {
        GUILayout.Label("API Be�ll�t�sok", EditorStyles.boldLabel);
        apiBaseUrl = EditorGUILayout.TextField("API base URL", apiBaseUrl);
        glbFileName = EditorGUILayout.TextField("GLB f�jl neve", glbFileName);
        jsonFileName = EditorGUILayout.TextField("JSON f�jl neve", jsonFileName);

        // Let�lt�s gomb megnyom�sakor ind�tjuk a let�lt�st
        if (GUILayout.Button("Let�lt�s �s import�l�s"))
        {
            DownloadFile(glbFileName);
            DownloadFile(jsonFileName);
            AssetDatabase.Refresh(); // Assetek friss�t�se Unity-ben
            Debug.Log("Let�lt�s �s import�l�s k�sz.");
        }
    }

    // Egy adott f�jl let�lt�se az API szerverr�l
    private void DownloadFile(string fileName)
    {
        string url = $"{apiBaseUrl}/download/{fileName}";
        string fullPath = Path.Combine(savePath, fileName);

        // Ha m�r l�tezik a f�jl, r�k�rdez�nk, hogy fel�l�rjuk-e
        if (File.Exists(fullPath))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "F�jl m�r l�tezik",
                $"{fileName} m�r l�tezik a mapp�ban.\nFel�l szeretn�d �rni?",
                "Igen, fel�l�rom",
                "Nem, kihagyom");

            if (!overwrite)
            {
                Debug.Log($"Let�lt�s kihagyva: {fileName} m�r l�tezik.");
                return;
            }
        }

        try
        {
            // HTTP GET k�r�s k�ld�se a f�jl el�r�s�re
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    using (var fileStream = File.Create(fullPath))
                    {
                        stream.CopyTo(fileStream); // Let�lt�tt tartalom ment�se
                        Debug.Log($"Let�ltve: {fileName}");
                    }
                }
            }
        }
        catch (WebException ex)
        {
            // Ha a f�jl nem tal�lhat� vagy m�s hiba t�rt�nik
            if (ex.Response is HttpWebResponse errorResponse && errorResponse.StatusCode == HttpStatusCode.NotFound)
            {
                Debug.LogWarning($"Nincs ilyen felt�lt�tt f�jl: {fileName}");
            }
            else
            {
                Debug.LogError($"Hiba t�rt�nt a let�lt�s sor�n: {fileName} - {ex.Message}");
            }
        }
    }
}
