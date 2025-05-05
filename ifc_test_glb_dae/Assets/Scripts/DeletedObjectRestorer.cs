using System.Collections.Generic;
using UnityEngine;

// T�r�ltnek jel�lt objektumok automatikus elrejt�se indul�skor.
public class DeletedObjectRestorer : MonoBehaviour
{
    // J�t�k indul�sakor lefut
    void Start()
    {
        HideMarkedObjects();
    }

    // T�r�ltk�nt megjel�lt objektumok elrejt�se
    void HideMarkedObjects()
    {
        // Lek�rj�k a t�rolt t�r�lt ID-k list�j�t
        List<string> deletedIds = DeletedIdManager.GetDeletedIds();

        // Ha nincs semmi t�r�lt ID, nincs teend�
        if (deletedIds == null || deletedIds.Count == 0)
            return;

        // V�gigiter�lunk az �sszes gyermek objektumon
        foreach (Transform child in transform)
        {
            // Ha a gyermek neve szerepel a t�r�lt ID-k k�z�tt
            if (deletedIds.Contains(child.name))
            {
                // Ha l�tezik a GameObject, elrejtj�k
                if (child.gameObject != null)
                {
                    child.gameObject.SetActive(false);

                    // Konzolra ki�rjuk, hogy elrejtett�k
                    Debug.Log($"Play Mode ind�t�skor elrejtve GameObject: {child.name}");
                }
            }
        }
    }
}
