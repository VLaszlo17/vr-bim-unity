using UnityEngine;
using UnityEngine.UI;

public class TogglePlaneSimple : MonoBehaviour
{
    public Toggle planeToggle;         // A Toggle, amit figyel�nk
    public GameObject planeObject;     // A Plane objektum, amit akt�v/inakt�v �llapotra �ll�tunk

    private void Start()
    {
        if (planeToggle != null && planeObject != null)
        {
            planeToggle.onValueChanged.AddListener(OnToggleChanged);

            // Indul�skor az aktu�lis �llapot szerint �ll�tjuk be
            planeObject.SetActive(planeToggle.isOn);
        }
        else
        {
            Debug.LogWarning("[TogglePlaneSimple] Toggle vagy Plane Object nincs be�ll�tva!");
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        if (planeObject != null)
        {
            planeObject.SetActive(isOn);
            Debug.Log($"[TogglePlaneSimple] Plane akt�v �llapota: {isOn}");
        }
    }
}
