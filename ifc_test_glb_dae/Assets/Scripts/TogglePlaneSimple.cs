using UnityEngine;
using UnityEngine.UI;

public class TogglePlaneSimple : MonoBehaviour
{
    public Toggle planeToggle;         // A Toggle, amit figyelünk
    public GameObject planeObject;     // A Plane objektum, amit aktív/inaktív állapotra állítunk

    private void Start()
    {
        if (planeToggle != null && planeObject != null)
        {
            planeToggle.onValueChanged.AddListener(OnToggleChanged);

            // Induláskor az aktuális állapot szerint állítjuk be
            planeObject.SetActive(planeToggle.isOn);
        }
        else
        {
            Debug.LogWarning("[TogglePlaneSimple] Toggle vagy Plane Object nincs beállítva!");
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        if (planeObject != null)
        {
            planeObject.SetActive(isOn);
            Debug.Log($"[TogglePlaneSimple] Plane aktív állapota: {isOn}");
        }
    }
}
