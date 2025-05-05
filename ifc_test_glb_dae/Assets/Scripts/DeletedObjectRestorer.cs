using System.Collections.Generic;
using UnityEngine;

// Töröltnek jelölt objektumok automatikus elrejtése induláskor.
public class DeletedObjectRestorer : MonoBehaviour
{
    // Játék indulásakor lefut
    void Start()
    {
        HideMarkedObjects();
    }

    // Töröltként megjelölt objektumok elrejtése
    void HideMarkedObjects()
    {
        // Lekérjük a tárolt törölt ID-k listáját
        List<string> deletedIds = DeletedIdManager.GetDeletedIds();

        // Ha nincs semmi törölt ID, nincs teendõ
        if (deletedIds == null || deletedIds.Count == 0)
            return;

        // Végigiterálunk az összes gyermek objektumon
        foreach (Transform child in transform)
        {
            // Ha a gyermek neve szerepel a törölt ID-k között
            if (deletedIds.Contains(child.name))
            {
                // Ha létezik a GameObject, elrejtjük
                if (child.gameObject != null)
                {
                    child.gameObject.SetActive(false);

                    // Konzolra kiírjuk, hogy elrejtettük
                    Debug.Log($"Play Mode indításkor elrejtve GameObject: {child.name}");
                }
            }
        }
    }
}
