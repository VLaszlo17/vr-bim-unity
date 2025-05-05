using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Automatikusan hozz�ad�s az �sszes gyermek GameObject-hez.
public class AddComponents : MonoBehaviour
{
    // A j�t�k indul�sakor lefut
    void Start()
    {
        AddComponentsRecursively(transform);
    }

    // Rekurz�van v�gigmegy�nk minden gyermek Transform-on, �s sz�ks�ges komponenseket adunk hozz�
    void AddComponentsRecursively(Transform parent)
    {
        // V�gigiter�lunk az �sszes k�zvetlen gyermeken
        foreach (Transform child in parent)
        {
            // Ha van MeshFilter �s nincs m�g MeshCollider, akkor hozz�adunk egyet
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter != null && child.GetComponent<MeshCollider>() == null)
            {
                // �j MeshCollider komponens hozz�ad�sa
                MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                // Nem �ll�tjuk convex-re, hogy pontos maradjon az �tk�z�s
                meshCollider.convex = false;
            }

            // Ha m�g nincs XR Interactable komponens, hozz�adunk egyet
            if (child.GetComponent<XRBaseInteractable>() == null)
            {
                // Egyszer� XR interakci� kezel� hozz�ad�sa
                child.gameObject.AddComponent<XRSimpleInteractable>();
            }

            // Ha m�g nincs Outline komponens, hozz�adunk egyet
            if (child.GetComponent<Outline>() == null)
            {
                // �j Outline komponens l�trehoz�sa
                Outline outline = child.gameObject.AddComponent<Outline>();
                outline.enabled = false; // Alapb�l nem lesz akt�v
                outline.OutlineMode = Outline.Mode.OutlineAll; // Minden �lt k�rberajzol
                outline.OutlineColor = Color.green; // Z�ld sz�n� lesz
                outline.OutlineWidth = 2f; // V�kony vonalvastags�g
            }

            // Rekurz�v h�v�s a k�vetkez� gyermekre
            AddComponentsRecursively(child);
        }
    }
}

