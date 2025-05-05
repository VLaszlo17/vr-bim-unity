using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

// VR-ban a függõleges (fel-le) mozgást kezelõ osztály.
public class VerticalLocomotion : MonoBehaviour
{
    public InputActionProperty verticalInput; // Fel-le mozgáshoz használt bemeneti adat (joystick)
    public Transform xrRig; // A teljes XR Rig (játékos pozíciója és iránya)
    public float verticalSpeed = 1f; // Függõleges mozgás sebessége

    // Minden frame-ben frissíti a függõleges mozgást
    void Update()
    {
        // Ha nincs megadva XR Rig, nem mozgunk
        if (xrRig == null)
            return;

        // Bemeneti érték lekérése (joystick X-Y tengelyek)
        Vector2 input = verticalInput.action.ReadValue<Vector2>();

        // Csak ha az Y tengelyen van elég bemenet (fel-le irányban)
        if (Mathf.Abs(input.y) > 0.1f)
        {
            // XR Rig pozíciójának függõleges frissítése
            xrRig.position += new Vector3(0, input.y * verticalSpeed * Time.deltaTime, 0);
        }
    }
}
