using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;
    public AudioSource noteAudioSource;
    public AudioClip alertClip;


    [Header("HUD Elements")]
    public GameObject noteHUD;
    public Image noteImageUI;
    public TMP_Text noteTextUI;
    public TMP_Text noteCommentUI;
    public TMP_Text pressEToCloseText;


    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;
        noteHUD.SetActive(false);
    }

    public void OpenNote(NoteItem note)
    {
        if (isOpen) return;

        isOpen = true;
        noteHUD.SetActive(true);
        Object.FindFirstObjectByType<PlayerInteraction>().enabled = false;

        if (note.noteIndex == 0)
        {
            // Sonido de alerta sin AudioManager
            noteAudioSource.PlayOneShot(alertClip);

            // Activar enemigo
            EnemyController.Instance.SpawnEnemy();
        }


        // Rellenar UI
        noteImageUI.sprite = note.noteImage;
        noteTextUI.text = note.noteText;
        noteCommentUI.text = note.noteComment;

        pressEToCloseText.text = "Presiona E para cerrar";
        pressEToCloseText.gameObject.SetActive(true);


        Object.FindFirstObjectByType<PlayerInteraction>()?.HidePrompt();

        // Pausar controles
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }


    public void CloseNote()
    {
        isOpen = false;
        noteHUD.SetActive(false);

        // Restaurar juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        pressEToCloseText.gameObject.SetActive(false);

        Object.FindFirstObjectByType<PlayerInteraction>().enabled = true;


    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.E))
        {
            CloseNote();
        }
    }
}
