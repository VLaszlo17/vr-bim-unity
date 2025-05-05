using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Siccity.GLTFUtility;

// GLB modellek letöltését és betöltését kezelõ osztály
public class GlbModelDownloader : MonoBehaviour
{
    public string apiBaseUrl = "http://127.0.0.1:8000"; // API szerver alap URL-je
    public Transform buttonParent; // Ide kerülnek a generált gombok (pl. ScrollView Content)
    public Button buttonPrefab;    // A gomb prefab amit létrehozunk
    public GameObject loadingCanvas; // A betöltés közbeni UI panel, amit eltüntetünk választás után

    // Játék indulásakor automatikusan elindítja a GLB fájlok listázását
    void Start()
    {
        StartCoroutine(GetGlbFileList());
    }

    // GLB fájlok listájának lekérése az API szerverrõl
    IEnumerator GetGlbFileList()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{apiBaseUrl}/files/glb");
        yield return request.SendWebRequest();

        // Hibaellenõrzés
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Lista lekérés hiba: " + request.error);
            yield break;
        }

        // JSON válasz feldolgozása
        var json = request.downloadHandler.text;
        var fileList = JsonUtility.FromJson<GlbFileListWrapper>(json);

        // Minden fájlhoz létrehozunk egy gombot
        foreach (string fileName in fileList.glb_files)
        {
            string cleanName = Path.GetFileNameWithoutExtension(fileName);

            Button button = Instantiate(buttonPrefab, buttonParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = cleanName;

            button.onClick.AddListener(() => OnFileSelected(fileName));
        }
    }

    // Amikor egy fájl gombjára kattintanak
    void OnFileSelected(string fileName)
    {
        StartCoroutine(DownloadAndLoadModel(fileName));
    }

    // GLB modell letöltése, betöltése, metaadatok hozzáadása
    IEnumerator DownloadAndLoadModel(string fileName)
    {
        // Betöltéskor eltüntetjük a UI-t
        loadingCanvas.SetActive(false);

        // --- 1. GLB fájl letöltése ---
        string glbUrl = $"{apiBaseUrl}/download/{fileName}";
        UnityWebRequest glbRequest = UnityWebRequest.Get(glbUrl);
        yield return glbRequest.SendWebRequest();

        if (glbRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("GLB fájl letöltési hiba: " + glbRequest.error);
            yield break;
        }

        // GLB adatok fájlba mentése
        byte[] glbData = glbRequest.downloadHandler.data;
        string tempGlbPath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(tempGlbPath, glbData);

        // GLB fájl betöltése jelenetbe
        ImportSettings importSettings = new ImportSettings();
        GameObject loadedModel = Importer.LoadFromFile(tempGlbPath, importSettings);
        loadedModel.transform.position = Vector3.zero;
        loadedModel.name = Path.GetFileNameWithoutExtension(fileName);

        // --- 2. JSON metaadatok letöltése ---
        string jsonFileName = Path.GetFileNameWithoutExtension(fileName) + ".json";
        string jsonUrl = $"{apiBaseUrl}/download/{jsonFileName}";
        UnityWebRequest jsonRequest = UnityWebRequest.Get(jsonUrl);
        yield return jsonRequest.SendWebRequest();

        if (jsonRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("JSON fájl letöltési hiba: " + jsonRequest.error);
            yield break;
        }

        string jsonText = jsonRequest.downloadHandler.text;

        // --- Metaadatok betöltése ---
        var metadataLoader = loadedModel.AddComponent<MetadataLoader>();
        metadataLoader.LoadFromJsonString(jsonText);

        // --- Szükséges komponensek automatikus hozzáadása ---
        loadedModel.AddComponent<MetadataAttacher>();
        loadedModel.AddComponent<AddComponents>();

        var teleportAreaRestorer = loadedModel.AddComponent<TeleportAreaRestorer>();
        var teleportProvider = GameObject.FindObjectOfType<UnityEngine.XR.Interaction.Toolkit.TeleportationProvider>();
        if (teleportProvider != null)
        {
            teleportAreaRestorer.teleportationProvider = teleportProvider;
        }
        teleportAreaRestorer.blockingTeleportReticle = GameObject.Find("Blocking Teleport Reticle");

        loadedModel.AddComponent<DeletedObjectRestorer>();

        // --- RestoreAllObjects parentTransform beállítása ---
        var restorer = GameObject.FindObjectOfType<RestoreAllObjects>();
        if (restorer != null)
        {
            restorer.parentTransform = loadedModel.transform;
            Debug.Log($"[DEBUG] RestoreAllObjects parentTransform beállítva: {restorer.parentTransform?.name}");
        }
        else
        {
            Debug.LogWarning("[DEBUG] RestoreAllObjects nem található a jelenetben!");
        }
    }

    // Segédosztály a JSON fájlok listájának deszerializálásához
    [System.Serializable]
    private class GlbFileListWrapper
    {
        public List<string> glb_files;
    }
}
