using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.XR.Interaction.Toolkit.BaseTeleportationInteractable;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

// Teleport�lhat� ter�letek hozz�ad�s�t, elt�vol�t�s�t �s kezel�s�t v�gz� oszt�ly.
[ExecuteAlways] // Editorban is fusson
public class TeleportAreaManager : MonoBehaviour
{
    public GameObject targetObject; // Az aktu�lisan kijel�lt GameObject
    public Button addButton; // TeleportArea hozz�ad� gomb
    public Button removeButton; // TeleportArea elt�vol�t� gomb
    public TeleportationProvider teleportationProvider; // Teleport�ci�t v�gz� komponens
    public GameObject blockingTeleportReticle; // Egyedi reticle (pl. blokkol� teleport�ci� ikon)

    // Indul�skor esem�nykezel�k be�ll�t�sa a gombokra
    private void Start()
    {
        addButton.onClick.AddListener(AddTeleportArea);
        removeButton.onClick.AddListener(RemoveTeleportArea);
    }

    // Kiv�lasztott target be�ll�t�sa
    public void SetTarget(GameObject target)
    {
        targetObject = target;
    }

    // TeleportArea komponens hozz�ad�sa a targethez
    private void AddTeleportArea()
    {
        if (targetObject == null)
            return;

        // Megl�v� XR interakci�s komponensek t�rl�se
        var interactables = targetObject.GetComponents<XRBaseInteractable>();
        foreach (var interactable in interactables)
        {
#if UNITY_EDITOR
            DestroyImmediate(interactable, true);
#else
            Destroy(interactable);
#endif
        }

        // TeleportAreaWithFade komponens hozz�ad�sa, ha m�g nincs
        var teleportArea = targetObject.GetComponent<TeleportAreaWithFade>();
        if (teleportArea == null)
        {
            teleportArea = targetObject.AddComponent<TeleportAreaWithFade>();
        }

        // TeleportArea alapbe�ll�t�sai
        teleportArea.interactionManager = FindObjectOfType<XRInteractionManager>();
        teleportArea.interactionLayers = InteractionLayerMask.GetMask("Default");
        teleportArea.distanceCalculationMode = BaseTeleportationInteractable.DistanceCalculationMode.ColliderPosition;
        teleportArea.teleportTrigger = TeleportTrigger.OnSelectExited;
        teleportArea.teleportationProvider = teleportationProvider;
        teleportArea.customReticle = blockingTeleportReticle;

        // Collider biztos�t�sa
        var collider = targetObject.GetComponent<Collider>();
        if (collider == null)
        {
            collider = targetObject.AddComponent<BoxCollider>();
        }
        else
        {
            // Ha MeshCollider, convex-re �ll�tjuk ha nem az
            if (collider is MeshCollider meshCollider)
            {
                if (!meshCollider.convex)
                {
                    meshCollider.convex = true;
                }
            }
        }
        collider.isTrigger = false;

        // Mentj�k az �j teleport�lhat� ID-t
        SaveTeleportableId(targetObject.name);
    }

    // �j teleport�lhat� objektum ID-j�nak ment�se JSON-ba
    private void SaveTeleportableId(string id)
    {
        string path = Application.persistentDataPath + "/teleportable_ids.json";
        List<string> ids = new List<string>();

        // Ha l�tezik a f�jl, beolvassuk
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            ids = JsonUtility.FromJson<IdList>(json).ids;
        }

        // �j ID hozz�ad�sa, ha m�g nincs benne
        if (!ids.Contains(id))
        {
            ids.Add(id);
            IdList idList = new IdList { ids = ids };
            string newJson = JsonUtility.ToJson(idList, true);
            System.IO.File.WriteAllText(path, newJson);
        }
    }

    [System.Serializable]
    private class IdList
    {
        public List<string> ids; // ID-k list�ja
    }

    // TeleportArea komponens elt�vol�t�sa a targetr�l
    private void RemoveTeleportArea()
    {
        if (targetObject == null)
            return;

        var teleportArea = targetObject.GetComponent<TeleportAreaWithFade>();
        if (teleportArea != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(teleportArea, true);
#else
            Destroy(teleportArea);
#endif
            // T�r�lj�k az ID-t is a list�b�l
            RemoveTeleportableId(targetObject.name);
            Debug.Log($"TeleportAreaWithFade elt�vol�tva �s ID t�r�lve: {targetObject.name}");
        }
    }

    // Teleport�lhat� ID elt�vol�t�sa a JSON f�jlb�l
    private void RemoveTeleportableId(string id)
    {
        string path = Application.persistentDataPath + "/teleportable_ids.json";

        if (!System.IO.File.Exists(path))
            return;

        string json = System.IO.File.ReadAllText(path);
        IdList idList = JsonUtility.FromJson<IdList>(json);

        if (idList == null || idList.ids == null)
            return;

        if (idList.ids.Contains(id))
        {
            idList.ids.Remove(id);

            if (idList.ids.Count == 0)
            {
                // Ha nincs t�bb ID, t�r�lj�k a f�jlt
                System.IO.File.Delete(path);
                Debug.Log("Mivel �res lett a teleport�lhat� lista, a teleportable_ids.json t�r�lve lett!");
            }
            else
            {
                // Ha maradt ID, �jra�rjuk
                string newJson = JsonUtility.ToJson(idList, true);
                System.IO.File.WriteAllText(path, newJson);
                Debug.Log($"ID t�r�lve a teleportable_ids.json-b�l: {id}");
            }
        }
    }

    // �sszes teleport�lhat� objektum vissza�ll�t�sa (TeleportArea elt�vol�t�sa)
    public void ResetTeleportableObjects()
    {
        string path = Application.persistentDataPath + "/teleportable_ids.json";

        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning("Nincs teleportable_ids.json f�jl, nincs mit t�r�lni.");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        IdList idList = JsonUtility.FromJson<IdList>(json);

        if (idList == null || idList.ids == null || idList.ids.Count == 0)
        {
            Debug.LogWarning("�res teleportable lista, nincs mit t�r�lni.");
            return;
        }

        foreach (string id in idList.ids)
        {
            GameObject obj = GameObject.Find(id);
            if (obj != null)
            {
                var teleportArea = obj.GetComponent<TeleportAreaWithFade>();
                if (teleportArea != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(teleportArea, true);
#else
                    Destroy(teleportArea);
#endif
                    Debug.Log($"TeleportAreaWithFade komponens t�r�lve: {id}");
                }
                else
                {
                    Debug.LogWarning($"Nem volt TeleportAreaWithFade komponens ezen: {id}");
                }
            }
            else
            {
                Debug.LogWarning($"Nem tal�lhat� GameObject ezzel az ID-val: {id}");
            }
        }

        // F�jl t�rl�se a v�g�n
        System.IO.File.Delete(path);
        Debug.Log("teleportable_ids.json f�jl t�r�lve, mert minden TeleportAreaWithFade komponens elt�vol�tva.");
    }
}
