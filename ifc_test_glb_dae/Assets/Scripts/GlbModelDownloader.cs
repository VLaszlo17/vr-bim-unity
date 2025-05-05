using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Siccity.GLTFUtility;

// GLB modellek let�lt�s�t �s bet�lt�s�t kezel� oszt�ly
public class GlbModelDownloader : MonoBehaviour
{
    public string apiBaseUrl = "http://127.0.0.1:8000"; // API szerver alap URL-je
    public Transform buttonParent; // Ide ker�lnek a gener�lt gombok (pl. ScrollView Content)
    public Button buttonPrefab;    // A gomb prefab amit l�trehozunk
    public GameObject loadingCanvas; // A bet�lt�s k�zbeni UI panel, amit elt�ntet�nk v�laszt�s ut�n

    // J�t�k indul�sakor automatikusan elind�tja a GLB f�jlok list�z�s�t
    void Start()
    {
        StartCoroutine(GetGlbFileList());
    }

    // GLB f�jlok list�j�nak lek�r�se az API szerverr�l
    IEnumerator GetGlbFileList()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{apiBaseUrl}/files/glb");
        yield return request.SendWebRequest();

        // Hibaellen�rz�s
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Lista lek�r�s hiba: " + request.error);
            yield break;
        }

        // JSON v�lasz feldolgoz�sa
        var json = request.downloadHandler.text;
        var fileList = JsonUtility.FromJson<GlbFileListWrapper>(json);

        // Minden f�jlhoz l�trehozunk egy gombot
        foreach (string fileName in fileList.glb_files)
        {
            string cleanName = Path.GetFileNameWithoutExtension(fileName);

            Button button = Instantiate(buttonPrefab, buttonParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = cleanName;

            button.onClick.AddListener(() => OnFileSelected(fileName));
        }
    }

    // Amikor egy f�jl gombj�ra kattintanak
    void OnFileSelected(string fileName)
    {
        StartCoroutine(DownloadAndLoadModel(fileName));
    }

    // GLB modell let�lt�se, bet�lt�se, metaadatok hozz�ad�sa
    IEnumerator DownloadAndLoadModel(string fileName)
    {
        // Bet�lt�skor elt�ntetj�k a UI-t
        loadingCanvas.SetActive(false);

        // --- 1. GLB f�jl let�lt�se ---
        string glbUrl = $"{apiBaseUrl}/download/{fileName}";
        UnityWebRequest glbRequest = UnityWebRequest.Get(glbUrl);
        yield return glbRequest.SendWebRequest();

        if (glbRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("GLB f�jl let�lt�si hiba: " + glbRequest.error);
            yield break;
        }

        // GLB adatok f�jlba ment�se
        byte[] glbData = glbRequest.downloadHandler.data;
        string tempGlbPath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(tempGlbPath, glbData);

        // GLB f�jl bet�lt�se jelenetbe
        ImportSettings importSettings = new ImportSettings();
        GameObject loadedModel = Importer.LoadFromFile(tempGlbPath, importSettings);
        loadedModel.transform.position = Vector3.zero;
        loadedModel.name = Path.GetFileNameWithoutExtension(fileName);

        // --- 2. JSON metaadatok let�lt�se ---
        string jsonFileName = Path.GetFileNameWithoutExtension(fileName) + ".json";
        string jsonUrl = $"{apiBaseUrl}/download/{jsonFileName}";
        UnityWebRequest jsonRequest = UnityWebRequest.Get(jsonUrl);
        yield return jsonRequest.SendWebRequest();

        if (jsonRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("JSON f�jl let�lt�si hiba: " + jsonRequest.error);
            yield break;
        }

        string jsonText = jsonRequest.downloadHandler.text;

        // --- Metaadatok bet�lt�se ---
        var metadataLoader = loadedModel.AddComponent<MetadataLoader>();
        metadataLoader.LoadFromJsonString(jsonText);

        // --- Sz�ks�ges komponensek automatikus hozz�ad�sa ---
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

        // --- RestoreAllObjects parentTransform be�ll�t�sa ---
        var restorer = GameObject.FindObjectOfType<RestoreAllObjects>();
        if (restorer != null)
        {
            restorer.parentTransform = loadedModel.transform;
            Debug.Log($"[DEBUG] RestoreAllObjects parentTransform be�ll�tva: {restorer.parentTransform?.name}");
        }
        else
        {
            Debug.LogWarning("[DEBUG] RestoreAllObjects nem tal�lhat� a jelenetben!");
        }
    }

    // Seg�doszt�ly a JSON f�jlok list�j�nak deszerializ�l�s�hoz
    [System.Serializable]
    private class GlbFileListWrapper
    {
        public List<string> glb_files;
    }
}
