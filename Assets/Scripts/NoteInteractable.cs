using UnityEngine;

public class NoteInteractable : MonoBehaviour, IInteractable
{
    public NoteItem note;

    private void Start()
    {
        // Registrar esta nota en NoteProgress
        NoteProgress.Instance.RegisterNote(this);
    }

    public string GetPromptText()
    {
        return "Presiona E para leer la nota";
    }

    public void Interact()
    {
        // Seguridad: si no está desbloqueada, simplemente no pasa nada
        if (!NoteProgress.Instance.HasFoundPreviousNotes(note.noteIndex))
            return;

        // Abrir la nota en pantalla
        NoteManager.Instance.OpenNote(note);

        // Registrar progreso solo la PRIMERA VEZ
        if (NoteProgress.Instance.notesFound < note.noteIndex)
        {
            NoteProgress.Instance.AddNote();
            HUDNotesCounter.Instance.UpdateCounter();
        }

        // Desaparecer la nota del mundo al recogerla
        gameObject.SetActive(false);
    }
}
