using UnityEngine;

// A Canvas panel helyes poz�cion�l�s�t v�gz� oszt�ly a kamera el�tt VR-ban
public class CanvasPositioner : MonoBehaviour
{
    public Transform cameraTransform; // A kamera Transform referenci�ja (XR Rig kamer�ja)
    public float distanceFromCamera = 2.0f; // Milyen messze legyen a Canvas a kamer�t�l
    public float heightOffset = 2.0f; // F�gg�leges eltol�s a Canvas poz�ci�j�n

    // J�t�k indul�sakor lefut
    void Start()
    {
        // Ha nincs be�ll�tva a kamera Transform, megpr�b�ljuk automatikusan megtal�lni
        if (cameraTransform == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cameraTransform = cam.transform;
            }
        }

        // Canvas poz�ci�j�nak be�ll�t�sa
        PositionCanvas();
    }

    // Be�ll�tja a Canvas poz�ci�j�t a kamera el�tt
    void PositionCanvas()
    {
        if (cameraTransform == null)
            return;

        // A kamera el�re ir�ny�nak lek�r�se (csak v�zszintes komponens)
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        // Poz�ci� kisz�m�t�sa: kamera poz�ci�ja + el�re ir�ny * t�vols�g
        transform.position = cameraTransform.position + forward * distanceFromCamera;
        // F�gg�leges eltol�s hozz�ad�sa
        transform.position += new Vector3(0, heightOffset, 0);

        // Canvas ir�ny�t�sa a kamera fel�
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0); // Megford�tjuk, hogy helyesen n�zzen a j�t�kos fel�
    }
}
