using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public Camera cam;

    [Header("UI")]
    public TMPro.TextMeshProUGUI promptText;
    public GameObject interactIconRoot;

    private IInteractable currentInteractable;

    void Update()
    {
        DetectInteractable();
        HandleInteraction();
    }

    void DetectInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            currentInteractable = hit.collider.GetComponent<IInteractable>()
                              ?? hit.collider.GetComponentInParent<IInteractable>();

            if (currentInteractable != null)
            {
                promptText.text = currentInteractable.GetPromptText();
                promptText.enabled = true;
                if (interactIconRoot != null) interactIconRoot.SetActive(true);
                return;
            }
        }

        // No interactable
        currentInteractable = null;
        promptText.enabled = false;
        if (interactIconRoot != null) interactIconRoot.SetActive(false);
    }

    void HandleInteraction()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
