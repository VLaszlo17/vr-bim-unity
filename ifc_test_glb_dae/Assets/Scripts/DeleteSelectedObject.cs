using UnityEngine;

// Kiv�lasztott GameObject t�rl�s��rt felel�s oszt�ly.
public class DeleteSelectedObject : MonoBehaviour
{
    public GameObject selectedObject; // Kiv�lasztott objektum referenci�ja
    public GameObject settingsCanvas; // Teleport be�ll�t� panel referenci�ja

    // A kiv�lasztott GameObject t�rl�se
    public void DeleteSelected()
    {
        // Ha nincs kiv�lasztott objektum, ki�rjuk �s kil�p�nk
        if (selectedObject == null)
        {
            Debug.LogWarning("Nincs kiv�lasztott objektum a t�rl�shez.");
            return;
        }

        // Hozz�adjuk az objektum nev�t a t�r�lt ID-k list�j�hoz
        DeletedIdManager.AddDeletedId(selectedObject.name);

        // Inakt�vv� tessz�k az objektumot
        selectedObject.SetActive(false);
        Debug.Log($"GameObject inakt�vv� t�ve �s ID elmentve: {selectedObject.name}");

        // T�r�lj�k a referenci�t
        selectedObject = null;

        // Ha meg volt nyitva a settingsCanvas, bez�rjuk
        if (settingsCanvas != null && settingsCanvas.activeSelf)
        {
            settingsCanvas.SetActive(false);
            Debug.Log("settingsCanvas bez�rva t�rl�skor.");
        }
    }
}
