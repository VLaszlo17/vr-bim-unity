using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit;

// Korábban teleportálhatóként megjelölt objektumok újratelepítését végzõ osztály.
public class TeleportAreaRestorer : MonoBehaviour
{
    public TeleportationProvider teleportationProvider; // Teleportációt kezelõ komponens
    public GameObject blockingTeleportReticle; // Teleportálás során használt egyedi reticle

    // Játék indulásakor lefut
    void Start()
    {
        RestoreTeleportAreas();
    }

    // TeleportArea komponensek visszaállítása a JSON fájl alapján
    void RestoreTeleportAreas()
    {
        string path = Application.persistentDataPath + "/teleportable_ids.json";

        // Ellenõrizzük, hogy létezik-e az ID lista
        if (!File.Exists(path))
        {
            Debug.LogWarning("Nincs teleportálható ID fájl.");
            return;
        }

        // ID lista betöltése
        string json = File.ReadAllText(path);
        IdList idList = JsonUtility.FromJson<IdList>(json);
        if (idList == null || idList.ids == null)
        {
            Debug.LogWarning("Üres ID lista.");
            return;
        }

        // Végigmegyünk az összes gyereken
        foreach (Transform child in transform)
        {
            // Ha az adott GameObject neve szerepel az ID listában
            if (idList.ids.Contains(child.name))
            {
                // Ha még nincs rajta TeleportAreaWithFade komponens, hozzáadjuk
                if (child.GetComponent<TeleportAreaWithFade>() == null)
                {
                    var teleportArea = child.gameObject.AddComponent<TeleportAreaWithFade>();

                    // TeleportArea alapbeállítások
                    teleportArea.interactionManager = FindObjectOfType<XRInteractionManager>();
                    teleportArea.interactionLayers = InteractionLayerMask.GetMask("Default");
                    teleportArea.distanceCalculationMode = BaseTeleportationInteractable.DistanceCalculationMode.ColliderPosition;
                    teleportArea.teleportTrigger = BaseTeleportationInteractable.TeleportTrigger.OnSelectExited;
                    teleportArea.teleportationProvider = teleportationProvider;
                    teleportArea.customReticle = blockingTeleportReticle;

                    // Collider biztosítása
                    var collider = child.GetComponent<Collider>();
                    if (collider == null)
                    {
                        collider = child.gameObject.AddComponent<BoxCollider>();
                    }
                    else if (collider is MeshCollider meshCollider && !meshCollider.convex)
                    {
                        meshCollider.convex = true;
                    }
                    collider.isTrigger = false;
                }
            }
        }

        Debug.Log("Teleportálható objektumok visszaállítva!");
    }

    [System.Serializable]
    private class IdList
    {
        public List<string> ids; // ID-k listája
    }
}
