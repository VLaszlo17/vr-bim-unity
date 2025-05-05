using UnityEngine;

// Kiválasztott GameObject törléséért felelõs osztály.
public class DeleteSelectedObject : MonoBehaviour
{
    public GameObject selectedObject; // Kiválasztott objektum referenciája
    public GameObject settingsCanvas; // Teleport beállító panel referenciája

    // A kiválasztott GameObject törlése
    public void DeleteSelected()
    {
        // Ha nincs kiválasztott objektum, kiírjuk és kilépünk
        if (selectedObject == null)
        {
            Debug.LogWarning("Nincs kiválasztott objektum a törléshez.");
            return;
        }

        // Hozzáadjuk az objektum nevét a törölt ID-k listájához
        DeletedIdManager.AddDeletedId(selectedObject.name);

        // Inaktívvá tesszük az objektumot
        selectedObject.SetActive(false);
        Debug.Log($"GameObject inaktívvá téve és ID elmentve: {selectedObject.name}");

        // Töröljük a referenciát
        selectedObject = null;

        // Ha meg volt nyitva a settingsCanvas, bezárjuk
        if (settingsCanvas != null && settingsCanvas.activeSelf)
        {
            settingsCanvas.SetActive(false);
            Debug.Log("settingsCanvas bezárva törléskor.");
        }
    }
}
