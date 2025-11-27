using UnityEngine;

public class NoteItem : MonoBehaviour
{
    public Sprite noteImage;        // la foto de la nota
    [TextArea(3, 10)]
    public string noteText;         // subtitulación
    [TextArea(3, 10)]
    public string noteComment;      // comentario del personaje
    public int noteIndex;

    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            NoteManager.Instance.OpenNote(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}
