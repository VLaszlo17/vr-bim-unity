using UnityEngine;
using UnityEngine.InputSystem;

// Egyszer� el�re-h�tra mozg�st kezel� oszt�ly VR-ban.
public class SimpleForwardLocomotion : MonoBehaviour
{
    public InputActionProperty moveInput; // A mozg�shoz haszn�lt bemeneti adat (joystick)
    public Transform xrRig; // A teljes XR Rig (j�t�kos poz�ci�ja �s ir�nya)
    public float moveSpeed = 1.5f; // Mozg�s sebess�ge

    // Minden frame-ben friss�ti a mozg�st
    void Update()
    {
        // Ha nincs megadva XR Rig, nem mozgunk
        if (xrRig == null)
            return;

        // Bemeneti �rt�k (joystick X-Y tengelyek)
        Vector2 input = moveInput.action.ReadValue<Vector2>();

        // Csak ha az Y ir�nyban (el�re-h�tra) van el�g er�s bemenet
        if (Mathf.Abs(input.y) > 0.1f)
        {
            // El�re ir�ny meghat�roz�sa (v�zszintes s�kban)
            Vector3 forward = xrRig.forward;
            forward.y = 0;
            forward.Normalize();

            // XR Rig poz�ci�j�nak friss�t�se a bemenet, sebess�g �s id� alapj�n
            xrRig.position += forward * input.y * moveSpeed * Time.deltaTime;
        }
    }
}
