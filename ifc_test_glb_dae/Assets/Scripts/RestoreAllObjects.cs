using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Törölt elemek visszaállítását és szükséges komponensek újrahúzását végzõ osztály.
public class RestoreAllObjects : MonoBehaviour
{
    public Transform parentTransform; // A szülõ GameObject

    // Az összes törölt elem visszaállítása és újrainicializálása
    public void RestoreAll()
    {
        // Töröljük az összes tárolt törölt ID-t
        DeletedIdManager.ClearDeletedIds();
        Debug.Log("Összes törölt ID törölve, modellek visszaállítva Play Mode következõ indítására.");

        // Ha van szülõ GameObject, végigmegyünk a gyerekeken
        if (parentTransform != null)
        {
            foreach (Transform child in parentTransform)
            {
                // Csak a jelenleg inaktív (elrejtett) gyerekeket állítjuk vissza
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true); // Újra láthatóvá tesszük
                    SetupRestoredObject(child.gameObject); // Komponenseket beállítjuk
                    Debug.Log($"Visszaállítva és komponensek újra beállítva: {child.name}");
                }
            }
        }
    }

    // Egy visszaállított GameObject-hez szükséges komponensek hozzáadása
    private void SetupRestoredObject(GameObject obj)
    {
        // MeshCollider hozzáadása, ha még nincs
        var meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = obj.AddComponent<MeshCollider>();
            meshCollider.convex = false; // Nem convex, hogy pontos legyen az ütközés
        }

        // XRSimpleInteractable hozzáadása, ha még nincs
        var interactable = obj.GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = obj.AddComponent<XRSimpleInteractable>();
            interactable.interactionManager = FindObjectOfType<XRInteractionManager>(); // Interaction Manager hozzákapcsolása
            interactable.interactionLayers = InteractionLayerMask.GetMask("Default"); // Default réteg használata
            interactable.selectMode = InteractableSelectMode.Single; // Egyes kiválasztási mód
            interactable.focusMode = InteractableFocusMode.Single; // Egyes fókusz mód
        }

        // Outline hozzáadása, ha még nincs
        var outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            outline = obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll; // Teljes kontúr
            outline.OutlineColor = Color.green; // Zöld szín
            outline.OutlineWidth = 2f; // Vékony kontúr
            outline.enabled = false; // Alapból kikapcsolva
        }
    }

    // Új parentTransform beállítása dinamikusan
    public void RefreshParent(Transform newParent)
    {
        parentTransform = newParent;
        Debug.Log($"RestoreAllObjects: új parentTransform beállítva: {newParent.name}");
    }

}
