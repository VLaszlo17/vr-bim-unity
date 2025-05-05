using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

// VR-ban a f�gg�leges (fel-le) mozg�st kezel� oszt�ly.
public class VerticalLocomotion : MonoBehaviour
{
    public InputActionProperty verticalInput; // Fel-le mozg�shoz haszn�lt bemeneti adat (joystick)
    public Transform xrRig; // A teljes XR Rig (j�t�kos poz�ci�ja �s ir�nya)
    public float verticalSpeed = 1f; // F�gg�leges mozg�s sebess�ge

    // Minden frame-ben friss�ti a f�gg�leges mozg�st
    void Update()
    {
        // Ha nincs megadva XR Rig, nem mozgunk
        if (xrRig == null)
            return;

        // Bemeneti �rt�k lek�r�se (joystick X-Y tengelyek)
        Vector2 input = verticalInput.action.ReadValue<Vector2>();

        // Csak ha az Y tengelyen van el�g bemenet (fel-le ir�nyban)
        if (Mathf.Abs(input.y) > 0.1f)
        {
            // XR Rig poz�ci�j�nak f�gg�leges friss�t�se
            xrRig.position += new Vector3(0, input.y * verticalSpeed * Time.deltaTime, 0);
        }
    }
}
