using UnityEngine;
using UnityEngine.UI;

// BIM panelek kezel�s��rt felel�s oszt�ly.
public class BimPanelManager : MonoBehaviour
{
    [Header("Panelek")]
    public GameObject bimElementInformationCanvas;
    public GameObject bimTeleportationSettingsCanvas;

    [Header("Plane kezel�s")]
    public GameObject planeObject;
    public Toggle usePlaneToggle;

    // J�t�k indul�sakor lefut
    void Start()
    {
        // Indul�skor biztosan kikapcsoljuk a teleport�ci�s panelt
        if (bimTeleportationSettingsCanvas != null)
            bimTeleportationSettingsCanvas.SetActive(false);
    }

    // �tv�lt�s a teleport�ci�s be�ll�t�sok panelre
    public void OpenTeleportationSettings()
    {
        if (bimElementInformationCanvas != null && bimTeleportationSettingsCanvas != null)
        {
            // A teleport panel �tveszi az inform�ci�s panel hely�t �s forgat�s�t
            bimTeleportationSettingsCanvas.transform.position = bimElementInformationCanvas.transform.position;
            bimTeleportationSettingsCanvas.transform.rotation = bimElementInformationCanvas.transform.rotation;

            // Inform�ci�s panel kikapcsol�sa, teleport�ci�s panel bekapcsol�sa
            bimElementInformationCanvas.SetActive(false);
            bimTeleportationSettingsCanvas.SetActive(true);
        }
    }

    // �tv�lt�s az elem inform�ci�s panelre
    public void OpenElementInformation()
    {
        if (bimElementInformationCanvas != null && bimTeleportationSettingsCanvas != null)
        {
            // Az inform�ci�s panel �tveszi a teleport panel hely�t �s forgat�s�t
            bimElementInformationCanvas.transform.position = bimTeleportationSettingsCanvas.transform.position;
            bimElementInformationCanvas.transform.rotation = bimTeleportationSettingsCanvas.transform.rotation;

            // Teleport�ci�s panel kikapcsol�sa, inform�ci�s panel bekapcsol�sa
            bimTeleportationSettingsCanvas.SetActive(false);
            bimElementInformationCanvas.SetActive(true);
        }
    }

    // Plane l�that�s�g�nak ki- �s bekapcsol�sa
    public void TogglePlaneVisibility()
    {
        if (planeObject != null)
        {
            // A plane l�that�s�g�t a kapcsol� (Toggle) �llapota alapj�n �ll�tjuk
            planeObject.SetActive(usePlaneToggle.isOn);
        }
    }
}
