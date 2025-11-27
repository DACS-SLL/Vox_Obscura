using UnityEngine;
using System.Collections.Generic;

public class NoteProgress : MonoBehaviour
{
    public static NoteProgress Instance;

    public int notesFound = 0;
    public int totalNotes = 6;

    private List<NoteInteractable> allNotes = new List<NoteInteractable>();
    private bool initialized = false;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterNote(NoteInteractable note)
    {
        allNotes.Add(note);

        // Cuando terminen de registrarse TODAS (por ejemplo 6)
        if (allNotes.Count == totalNotes && !initialized)
        {
            InitializeNotes();
        }
    }

    private void InitializeNotes()
    {
        initialized = true;

        foreach (var note in allNotes)
        {
            bool enable = (note.note.noteIndex == 1);
            note.gameObject.SetActive(enable);
        }

        Debug.Log("Notas inicializadas: solo la 1 está activa.");
    }

    public void AddNote()
    {
        notesFound++;

        Debug.Log($"Nota encontrada: {notesFound}/{totalNotes}");

        // Activar la siguiente nota
        foreach (var note in allNotes)
        {
            if (note.note.noteIndex == notesFound + 1)
            {
                note.gameObject.SetActive(true);
                Debug.Log($"Nota {note.note.noteIndex} ACTIVADA");
            }
        }

        if (notesFound == totalNotes)
        {
            HUDNotesCounter.Instance.ShowDoorUnlockedMessage();
        }
    }

    public bool HasFoundPreviousNotes(int noteIndex)
    {
        return notesFound >= (noteIndex - 1);
    }
}
