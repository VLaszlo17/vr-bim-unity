using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

// VR lézerrel kijelölt objektum metaadatainak megjelenítését és kezelését végző osztály.
public class MetadataSelector : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // Lézeres kijelöléshez használt XRRayInteractor komponens
    public ActionBasedController controller; // Kontroller kezelése ActionBasedController segítségével
    public float activationThreshold = 0.1f; // Aktiválási küszöbérték a gomb lenyomásához

    public TextMeshProUGUI metadataTextField; // A kijelölt elem metaadatait megjelenítő TextMeshPro UI szövegmező

    public GameObject canvasToShow; // A megjelenítendő információs panel (Canvas)
    public Transform cameraTransform; // Kamera pozíciójának referenciája
    public float canvasDistance = 1f; // Canvas távolsága a kamerától előre
    public float canvasHeight = -0.05f; // Canvas magasságának korrekciója
    public float canvasWidth = -0.2f; // Canvas oldalirányú (bal-jobb) pozíció eltolása

    private Outline currentOutline; // Az aktuálisan kijelölt objektum Outline komponense
    private GameObject currentTarget; // Az aktuálisan kijelölt GameObject

    public AudioSource audioSource; // Hangvisszajelzéshez használt AudioSource
    public AudioClip feedbackClip; // A visszajelzéshez használt AudioClip (pl. kattintás hang)

    public float hapticAmplitude = 0.5f; // Haptikus visszajelzés erőssége
    public float hapticDuration = 0.1f; // Haptikus visszajelzés időtartama másodpercben
    private float lastHapticTime = -1f; // Utolsó haptikus visszajelzés időpontja
    public float hapticCooldown = 0.2f; // Két haptikus visszajelzés közötti minimális idő (cooldown)

    public TeleportAreaManager teleportAreaManager; // TeleportAreaManager referencia a teleport célok kezeléséhez
    public DeleteSelectedObject deleteSelectedObject; // DeleteSelectedObject referencia a törlés funkció kezeléséhez

    public GameObject settingsCanvas; // A teleport beállító panel (Canvas), amit ki-be lehet kapcsolni


    // Minden frame-ben ellenőrzi a kijelölést és frissíti a megjelenítést
    void Update()
    {
        // Ha nincs szükséges komponens, kilépünk
        if (controller == null || rayInteractor == null || metadataTextField == null)
            return;

        // Ha nincs lenyomva az aktiváló gomb, kilépünk
        if (!IsActivationPressed(controller))
            return;

        // Ha éppen UI elemen vagyunk, nem csinálunk semmit
        if (rayInteractor.TryGetCurrentUIRaycastResult(out var uiHit) && uiHit.isValid)
            return;

        // 3D objektum eltalálása esetén
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            var metadata = hit.transform.GetComponent<MetadataDisplay>();
            if (metadata != null)
            {
                // Ha a teleport panel nyitva van, bezárjuk
                if (settingsCanvas != null && settingsCanvas.activeSelf)
                {
                    settingsCanvas.SetActive(false);
                    Debug.Log("settingsCanvas bezárva új kijelöléskor.");
                }

                // Metaadatok megjelenítése
                metadataTextField.text = metadata.GetFormattedMetadata();
                canvasToShow.SetActive(true);

                // Haptikus és hang visszajelzés
                SendHapticFeedback();
                PlayAudioFeedback();

                // Canvas pozícionálása a kamera előtt
                Vector3 forward = cameraTransform.forward;
                Vector3 left = cameraTransform.right;
                forward.y = 0;
                left.y = 0;
                forward.Normalize();
                left.Normalize();

                Vector3 targetPos = cameraTransform.position + forward * canvasDistance + left * canvasWidth;
                targetPos.y += canvasHeight;
                canvasToShow.transform.position = targetPos;
                canvasToShow.transform.LookAt(cameraTransform);
                canvasToShow.transform.Rotate(0, 180, 0);

                // Outline kiemelés kezelése
                if (currentOutline != null)
                    currentOutline.enabled = false;

                currentTarget = hit.transform.gameObject;
                currentOutline = currentTarget.GetComponent<Outline>();
                if (currentOutline != null)
                    currentOutline.enabled = true;

                // Target frissítése a teleport és törlés managerek felé
                SelectTarget(currentTarget);
            }
            else
            {
                // Ha nincs metaadat, üres üzenetet írunk ki
                metadataTextField.text = "Nincs metaadat ehhez az objektumhoz.";
                ClearOutline();

                SelectTarget(null);
            }
        }
    }

    // Megnézi, hogy az aktiváló gomb meg van-e nyomva
    bool IsActivationPressed(ActionBasedController controller)
    {
        return controller.activateAction.action.ReadValue<float>() > activationThreshold;
    }

    // Outline eltávolítása az aktuális célobjektumról
    public void ClearOutline()
    {
        if (currentOutline != null)
            currentOutline.enabled = false;

        currentOutline = null;
        currentTarget = null;
    }

    // Haptikus visszajelzés küldése a kontrollerre
    private void SendHapticFeedback()
    {
        if (controller != null)
        {
            if (Time.time - lastHapticTime > hapticCooldown)
            {
                controller.SendHapticImpulse(hapticAmplitude, hapticDuration);
                lastHapticTime = Time.time;
            }
        }
    }

    // Hang visszajelzés lejátszása
    private void PlayAudioFeedback()
    {
        if (audioSource != null && feedbackClip != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop(); // Leállítjuk az esetleges korábbi lejátszást

            audioSource.clip = feedbackClip;
            audioSource.Play();
        }
    }

    // A teleportációs és törlő menedzserek céljának beállítása
    private void SelectTarget(GameObject target)
    {
        if (teleportAreaManager != null)
        {
            teleportAreaManager.SetTarget(target);
        }

        if (deleteSelectedObject != null)
        {
            deleteSelectedObject.selectedObject = target;
        }
    }
}
