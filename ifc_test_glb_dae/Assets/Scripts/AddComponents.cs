using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Automatikusan hozzáadés az összes gyermek GameObject-hez.
public class AddComponents : MonoBehaviour
{
    // A játék indulásakor lefut
    void Start()
    {
        AddComponentsRecursively(transform);
    }

    // Rekurzívan végigmegyünk minden gyermek Transform-on, és szükséges komponenseket adunk hozzá
    void AddComponentsRecursively(Transform parent)
    {
        // Végigiterálunk az összes közvetlen gyermeken
        foreach (Transform child in parent)
        {
            // Ha van MeshFilter és nincs még MeshCollider, akkor hozzáadunk egyet
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter != null && child.GetComponent<MeshCollider>() == null)
            {
                // Új MeshCollider komponens hozzáadása
                MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                // Nem állítjuk convex-re, hogy pontos maradjon az ütközés
                meshCollider.convex = false;
            }

            // Ha még nincs XR Interactable komponens, hozzáadunk egyet
            if (child.GetComponent<XRBaseInteractable>() == null)
            {
                // Egyszerû XR interakció kezelõ hozzáadása
                child.gameObject.AddComponent<XRSimpleInteractable>();
            }

            // Ha még nincs Outline komponens, hozzáadunk egyet
            if (child.GetComponent<Outline>() == null)
            {
                // Új Outline komponens létrehozása
                Outline outline = child.gameObject.AddComponent<Outline>();
                outline.enabled = false; // Alapból nem lesz aktív
                outline.OutlineMode = Outline.Mode.OutlineAll; // Minden élt körberajzol
                outline.OutlineColor = Color.green; // Zöld színû lesz
                outline.OutlineWidth = 2f; // Vékony vonalvastagság
            }

            // Rekurzív hívás a következõ gyermekre
            AddComponentsRecursively(child);
        }
    }
}

