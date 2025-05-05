using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Lézer színének dinamikus változtatása UI fölé mutatáskor.
public class LaserColorChanger : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // Lézer interaktor
    public XRInteractorLineVisual lineVisual; // Lézer vizuális megjelenítése

    public Gradient defaultColor; // Alapértelmezett szín
    public Gradient uiHoverColor; // UI hover szín

    public LayerMask uiLayerMask; // UI réteg maszk

    // Minden frame-ben frissíti a lézer színét
    void Update()
    {
        // Ha nincs rayInteractor vagy lineVisual, nem csinálunk semmit
        if (rayInteractor == null || lineVisual == null)
            return;

        // Megpróbáljuk lekérni, hogy mire mutat most a lézer
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Ha a találat a UI rétegen van
            if (((1 << hit.collider.gameObject.layer) & uiLayerMask) != 0)
            {
                // Átváltjuk a lézer színét a UI hover színre
                lineVisual.validColorGradient = uiHoverColor;
                return;
            }
        }

        // Ha nem UI-ra mutat, visszaállítjuk az alap színt
        lineVisual.validColorGradient = defaultColor;
    }
}
