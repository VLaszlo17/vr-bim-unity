using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.XR.Interaction.Toolkit.BaseTeleportationInteractable;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

// Teleportálható területek hozzáadását, eltávolítását és kezelését végzõ osztály.
[ExecuteAlways] // Editorban is fusson
public class TeleportAreaManager : MonoBehaviour
{
    public GameObject targetObject; // Az aktuálisan kijelölt GameObject
    public Button addButton; // TeleportArea hozzáadó gomb
    public Button removeButton; // TeleportArea eltávolító gomb
    public TeleportationProvider teleportationProvider; // Teleportációt végzõ komponens
    public GameObject blockingTeleportReticle; // Egyedi reticle (pl. blokkoló teleportáció ikon)

    // Induláskor eseménykezelõk beállítása a gombokra
    private void Start()
    {
        addButton.onClick.AddListener(AddTeleportArea);
        removeButton.onClick.AddListener(RemoveTeleportArea);
    }

    // Kiválasztott target beállítása
    public void SetTarget(GameObject target)
    {
        targetObject = target;
    }

    // TeleportArea komponens hozzáadása a targethez
    private void AddTeleportArea()
    {
        if (targetObject == null)
            return;

        // Meglévõ XR interakciós komponensek törlése
        var interactables = targetObject.GetComponents<XRBaseInteractable>();
        foreach (var interactable in interactables)
        {
#if UNITY_EDITOR
            DestroyImmediate(interactable, true);
#else
            Destroy(interactable);
#endif
        }

        // TeleportAreaWithFade komponens hozzáadása, ha még nincs
        var teleportArea = targetObject.GetComponent<TeleportAreaWithFade>();
        if (teleportArea == null)
        {
            teleportArea = targetObject.AddComponent<TeleportAreaWithFade>();
        }

        // TeleportArea alapbeállításai
        teleportArea.interactionManager = FindObjectOfType<XRInteractionManager>();
        teleportArea.interactionLayers = InteractionLayerMask.GetMask("Default");
        teleportArea.distanceCalculationMode = BaseTeleportationInteractable.DistanceCalculationMode.ColliderPosition;
        teleportArea.teleportTrigger = TeleportTrigger.OnSelectExited;
        teleportArea.teleportationProvider = teleportationProvider;
        teleportArea.customReticle = blockingTeleportReticle;

        // Collider biztosítása
        var collider = targetObject.GetComponent<Collider>();
        if (collider == null)
        {
            collider = targetObject.AddComponent<BoxCollider>();
        }
        else
        {
            // Ha MeshCollider, convex-re állítjuk ha nem az
            if (collider is MeshCollider meshCollider)
            {
                if (!meshCollider.convex)
                {
                    meshCollider.convex = true;
                }
            }
        }
        collider.isTrigger = false;

        // Mentjük az új teleportálható ID-t
        SaveTeleportableId(targetObject.name);
    }

    // Új teleportálható objektum ID-jának mentése JSON-ba
    private void SaveTeleportableId(string id)
    {
        string path = Application.persistentDataPath + "/teleportable_ids.json";
        List<string> ids = new List<string>();

        // Ha létezik a fájl, beolvassuk
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            ids = JsonUtility.FromJson<IdList>(json).ids;
        }

        // Új ID hozzáadása, ha még nincs benne
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
        public List<string> ids; // ID-k listája
    }

    // TeleportArea komponens eltávolítása a targetrõl
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
            // Töröljük az ID-t is a listából
            RemoveTeleportableId(targetObject.name);
            Debug.Log($"TeleportAreaWithFade eltávolítva és ID törölve: {targetObject.name}");
        }
    }

    // Teleportálható ID eltávolítása a JSON fájlból
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
                // Ha nincs több ID, töröljük a fájlt
                System.IO.File.Delete(path);
                Debug.Log("Mivel üres lett a teleportálható lista, a teleportable_ids.json törölve lett!");
            }
            else
            {
                // Ha maradt ID, újraírjuk
                string newJson = JsonUtility.ToJson(idList, true);
                System.IO.File.WriteAllText(path, newJson);
                Debug.Log($"ID törölve a teleportable_ids.json-ból: {id}");
            }
        }
    }

    // Összes teleportálható objektum visszaállítása (TeleportArea eltávolítása)
    public void ResetTeleportableObjects()
    {
        string path = Application.persistentDataPath + "/teleportable_ids.json";

        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning("Nincs teleportable_ids.json fájl, nincs mit törölni.");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        IdList idList = JsonUtility.FromJson<IdList>(json);

        if (idList == null || idList.ids == null || idList.ids.Count == 0)
        {
            Debug.LogWarning("Üres teleportable lista, nincs mit törölni.");
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
                    Debug.Log($"TeleportAreaWithFade komponens törölve: {id}");
                }
                else
                {
                    Debug.LogWarning($"Nem volt TeleportAreaWithFade komponens ezen: {id}");
                }
            }
            else
            {
                Debug.LogWarning($"Nem található GameObject ezzel az ID-val: {id}");
            }
        }

        // Fájl törlése a végén
        System.IO.File.Delete(path);
        Debug.Log("teleportable_ids.json fájl törölve, mert minden TeleportAreaWithFade komponens eltávolítva.");
    }
}
