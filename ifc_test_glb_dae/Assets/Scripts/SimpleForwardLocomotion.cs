using UnityEngine;
using UnityEngine.InputSystem;

// Egyszerû elõre-hátra mozgást kezelõ osztály VR-ban.
public class SimpleForwardLocomotion : MonoBehaviour
{
    public InputActionProperty moveInput; // A mozgáshoz használt bemeneti adat (joystick)
    public Transform xrRig; // A teljes XR Rig (játékos pozíciója és iránya)
    public float moveSpeed = 1.5f; // Mozgás sebessége

    // Minden frame-ben frissíti a mozgást
    void Update()
    {
        // Ha nincs megadva XR Rig, nem mozgunk
        if (xrRig == null)
            return;

        // Bemeneti érték (joystick X-Y tengelyek)
        Vector2 input = moveInput.action.ReadValue<Vector2>();

        // Csak ha az Y irányban (elõre-hátra) van elég erõs bemenet
        if (Mathf.Abs(input.y) > 0.1f)
        {
            // Elõre irány meghatározása (vízszintes síkban)
            Vector3 forward = xrRig.forward;
            forward.y = 0;
            forward.Normalize();

            // XR Rig pozíciójának frissítése a bemenet, sebesség és idõ alapján
            xrRig.position += forward * input.y * moveSpeed * Time.deltaTime;
        }
    }
}
