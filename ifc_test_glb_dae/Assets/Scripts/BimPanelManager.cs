using UnityEngine;
using UnityEngine.UI;

// BIM panelek kezeléséért felelõs osztály.
public class BimPanelManager : MonoBehaviour
{
    [Header("Panelek")]
    public GameObject bimElementInformationCanvas;
    public GameObject bimTeleportationSettingsCanvas;

    [Header("Plane kezelés")]
    public GameObject planeObject;
    public Toggle usePlaneToggle;

    // Játék indulásakor lefut
    void Start()
    {
        // Induláskor biztosan kikapcsoljuk a teleportációs panelt
        if (bimTeleportationSettingsCanvas != null)
            bimTeleportationSettingsCanvas.SetActive(false);
    }

    // Átváltás a teleportációs beállítások panelre
    public void OpenTeleportationSettings()
    {
        if (bimElementInformationCanvas != null && bimTeleportationSettingsCanvas != null)
        {
            // A teleport panel átveszi az információs panel helyét és forgatását
            bimTeleportationSettingsCanvas.transform.position = bimElementInformationCanvas.transform.position;
            bimTeleportationSettingsCanvas.transform.rotation = bimElementInformationCanvas.transform.rotation;

            // Információs panel kikapcsolása, teleportációs panel bekapcsolása
            bimElementInformationCanvas.SetActive(false);
            bimTeleportationSettingsCanvas.SetActive(true);
        }
    }

    // Átváltás az elem információs panelre
    public void OpenElementInformation()
    {
        if (bimElementInformationCanvas != null && bimTeleportationSettingsCanvas != null)
        {
            // Az információs panel átveszi a teleport panel helyét és forgatását
            bimElementInformationCanvas.transform.position = bimTeleportationSettingsCanvas.transform.position;
            bimElementInformationCanvas.transform.rotation = bimTeleportationSettingsCanvas.transform.rotation;

            // Teleportációs panel kikapcsolása, információs panel bekapcsolása
            bimTeleportationSettingsCanvas.SetActive(false);
            bimElementInformationCanvas.SetActive(true);
        }
    }

    // Plane láthatóságának ki- és bekapcsolása
    public void TogglePlaneVisibility()
    {
        if (planeObject != null)
        {
            // A plane láthatóságát a kapcsoló (Toggle) állapota alapján állítjuk
            planeObject.SetActive(usePlaneToggle.isOn);
        }
    }
}
