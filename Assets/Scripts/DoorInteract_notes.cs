using UnityEngine;

public class DoorInteractable_notes : MonoBehaviour, IInteractable
{
    public Transform hinge;
    public float openAngle = 90f;
    public float speed = 5f;

    [Header("Requisitos")]
    public int requiredNotes = 6;

    // Audio
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip closeClip;
    public float audioPitchVariation = 0.02f;

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
        // --- BLOQUEO POR NOTAS ---
        if (NoteProgress.Instance.notesFound < requiredNotes)
        {
            Debug.Log("La puerta está bloqueada. Aún faltan notas.");
            return;
        }
        // ----------------------------

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
        if (NoteProgress.Instance.notesFound < requiredNotes)
            return "La puerta está bloqueada...";

        return isOpen ? "Cerrar puerta (E)" : "Abrir puerta (E)";
    }
}
