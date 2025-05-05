using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// L�zer sz�n�nek dinamikus v�ltoztat�sa UI f�l� mutat�skor.
public class LaserColorChanger : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // L�zer interaktor
    public XRInteractorLineVisual lineVisual; // L�zer vizu�lis megjelen�t�se

    public Gradient defaultColor; // Alap�rtelmezett sz�n
    public Gradient uiHoverColor; // UI hover sz�n

    public LayerMask uiLayerMask; // UI r�teg maszk

    // Minden frame-ben friss�ti a l�zer sz�n�t
    void Update()
    {
        // Ha nincs rayInteractor vagy lineVisual, nem csin�lunk semmit
        if (rayInteractor == null || lineVisual == null)
            return;

        // Megpr�b�ljuk lek�rni, hogy mire mutat most a l�zer
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Ha a tal�lat a UI r�tegen van
            if (((1 << hit.collider.gameObject.layer) & uiLayerMask) != 0)
            {
                // �tv�ltjuk a l�zer sz�n�t a UI hover sz�nre
                lineVisual.validColorGradient = uiHoverColor;
                return;
            }
        }

        // Ha nem UI-ra mutat, vissza�ll�tjuk az alap sz�nt
        lineVisual.validColorGradient = defaultColor;
    }
}
