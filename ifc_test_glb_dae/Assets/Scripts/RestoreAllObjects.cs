using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// T�r�lt elemek vissza�ll�t�s�t �s sz�ks�ges komponensek �jrah�z�s�t v�gz� oszt�ly.
public class RestoreAllObjects : MonoBehaviour
{
    public Transform parentTransform; // A sz�l� GameObject

    // Az �sszes t�r�lt elem vissza�ll�t�sa �s �jrainicializ�l�sa
    public void RestoreAll()
    {
        // T�r�lj�k az �sszes t�rolt t�r�lt ID-t
        DeletedIdManager.ClearDeletedIds();
        Debug.Log("�sszes t�r�lt ID t�r�lve, modellek vissza�ll�tva Play Mode k�vetkez� ind�t�s�ra.");

        // Ha van sz�l� GameObject, v�gigmegy�nk a gyerekeken
        if (parentTransform != null)
        {
            foreach (Transform child in parentTransform)
            {
                // Csak a jelenleg inakt�v (elrejtett) gyerekeket �ll�tjuk vissza
                if (!child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true); // �jra l�that�v� tessz�k
                    SetupRestoredObject(child.gameObject); // Komponenseket be�ll�tjuk
                    Debug.Log($"Vissza�ll�tva �s komponensek �jra be�ll�tva: {child.name}");
                }
            }
        }
    }

    // Egy vissza�ll�tott GameObject-hez sz�ks�ges komponensek hozz�ad�sa
    private void SetupRestoredObject(GameObject obj)
    {
        // MeshCollider hozz�ad�sa, ha m�g nincs
        var meshCollider = obj.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = obj.AddComponent<MeshCollider>();
            meshCollider.convex = false; // Nem convex, hogy pontos legyen az �tk�z�s
        }

        // XRSimpleInteractable hozz�ad�sa, ha m�g nincs
        var interactable = obj.GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = obj.AddComponent<XRSimpleInteractable>();
            interactable.interactionManager = FindObjectOfType<XRInteractionManager>(); // Interaction Manager hozz�kapcsol�sa
            interactable.interactionLayers = InteractionLayerMask.GetMask("Default"); // Default r�teg haszn�lata
            interactable.selectMode = InteractableSelectMode.Single; // Egyes kiv�laszt�si m�d
            interactable.focusMode = InteractableFocusMode.Single; // Egyes f�kusz m�d
        }

        // Outline hozz�ad�sa, ha m�g nincs
        var outline = obj.GetComponent<Outline>();
        if (outline == null)
        {
            outline = obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll; // Teljes kont�r
            outline.OutlineColor = Color.green; // Z�ld sz�n
            outline.OutlineWidth = 2f; // V�kony kont�r
            outline.enabled = false; // Alapb�l kikapcsolva
        }
    }

    // �j parentTransform be�ll�t�sa dinamikusan
    public void RefreshParent(Transform newParent)
    {
        parentTransform = newParent;
        Debug.Log($"RestoreAllObjects: �j parentTransform be�ll�tva: {newParent.name}");
    }

}
