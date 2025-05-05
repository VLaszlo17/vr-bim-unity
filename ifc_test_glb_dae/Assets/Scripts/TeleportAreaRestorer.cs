using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit;

// Kor�bban teleport�lhat�k�nt megjel�lt objektumok �jratelep�t�s�t v�gz� oszt�ly.
public class TeleportAreaRestorer : MonoBehaviour
{
    public TeleportationProvider teleportationProvider; // Teleport�ci�t kezel� komponens
    public GameObject blockingTeleportReticle; // Teleport�l�s sor�n haszn�lt egyedi reticle

    // J�t�k indul�sakor lefut
    void Start()
    {
        RestoreTeleportAreas();
    }

    // TeleportArea komponensek vissza�ll�t�sa a JSON f�jl alapj�n
    void RestoreTeleportAreas()
    {
        string path = Application.persistentDataPath + "/teleportable_ids.json";

        // Ellen�rizz�k, hogy l�tezik-e az ID lista
        if (!File.Exists(path))
        {
            Debug.LogWarning("Nincs teleport�lhat� ID f�jl.");
            return;
        }

        // ID lista bet�lt�se
        string json = File.ReadAllText(path);
        IdList idList = JsonUtility.FromJson<IdList>(json);
        if (idList == null || idList.ids == null)
        {
            Debug.LogWarning("�res ID lista.");
            return;
        }

        // V�gigmegy�nk az �sszes gyereken
        foreach (Transform child in transform)
        {
            // Ha az adott GameObject neve szerepel az ID list�ban
            if (idList.ids.Contains(child.name))
            {
                // Ha m�g nincs rajta TeleportAreaWithFade komponens, hozz�adjuk
                if (child.GetComponent<TeleportAreaWithFade>() == null)
                {
                    var teleportArea = child.gameObject.AddComponent<TeleportAreaWithFade>();

                    // TeleportArea alapbe�ll�t�sok
                    teleportArea.interactionManager = FindObjectOfType<XRInteractionManager>();
                    teleportArea.interactionLayers = InteractionLayerMask.GetMask("Default");
                    teleportArea.distanceCalculationMode = BaseTeleportationInteractable.DistanceCalculationMode.ColliderPosition;
                    teleportArea.teleportTrigger = BaseTeleportationInteractable.TeleportTrigger.OnSelectExited;
                    teleportArea.teleportationProvider = teleportationProvider;
                    teleportArea.customReticle = blockingTeleportReticle;

                    // Collider biztos�t�sa
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

        Debug.Log("Teleport�lhat� objektumok vissza�ll�tva!");
    }

    [System.Serializable]
    private class IdList
    {
        public List<string> ids; // ID-k list�ja
    }
}
