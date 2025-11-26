using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    public Transform hinge;
    public float openAngle = 90f;
    public float speed = 5f;

    // Audio
    public AudioSource audioSource;      // asignar el AudioSource del mismo GameObject
    public AudioClip openClip;
    public AudioClip closeClip;
    public float audioPitchVariation = 0.02f; // ligera variación

    private bool isOpen = false;

    void Update()
    {
        float target = isOpen ? openAngle : 0f;
        Vector3 rot = hinge.localEulerAngles;
        rot.y = Mathf.LerpAngle(rot.y, target, Time.deltaTime * speed);
        hinge.localEulerAngles = rot;
    }

    public void Interact()
    {
        isOpen = !isOpen;

        // Reproducir sonido
        if (audioSource != null)
        {
            audioSource.pitch = 1f + Random.Range(-audioPitchVariation, audioPitchVariation);
            audioSource.PlayOneShot(isOpen ? openClip : closeClip);
        }
    }

    public string GetPromptText()
    {
        return isOpen ? "Cerrar puerta (E)" : "Abrir puerta (E)";
    }
}
