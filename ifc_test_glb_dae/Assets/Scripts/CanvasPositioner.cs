using UnityEngine;

// A Canvas panel helyes pozícionálását végzõ osztály a kamera elõtt VR-ban
public class CanvasPositioner : MonoBehaviour
{
    public Transform cameraTransform; // A kamera Transform referenciája (XR Rig kamerája)
    public float distanceFromCamera = 2f; // Milyen messze legyen a Canvas a kamerától
    public float heightOffset = 0f; // Függõleges eltolás a Canvas pozícióján

    // Játék indulásakor lefut
    void Start()
    {
        // Ha nincs beállítva a kamera Transform, megpróbáljuk automatikusan megtalálni
        if (cameraTransform == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cameraTransform = cam.transform;
            }
        }

        // Canvas pozíciójának beállítása
        PositionCanvas();
    }

    // Beállítja a Canvas pozícióját a kamera elõtt
    void PositionCanvas()
    {
        if (cameraTransform == null)
            return;

        // A kamera elõre irányának lekérése (csak vízszintes komponens)
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        // Pozíció kiszámítása: kamera pozíciója + elõre irány * távolság
        transform.position = cameraTransform.position + forward * distanceFromCamera;
        // Függõleges eltolás hozzáadása
        transform.position += new Vector3(0, heightOffset, 0);

        // Canvas irányítása a kamera felé
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0); // Megfordítjuk, hogy helyesen nézzen a játékos felé
    }
}
