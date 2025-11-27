using UnityEngine;
using TMPro;

public class HUDNotesCounter : MonoBehaviour
{
    public static HUDNotesCounter Instance;

    [Header("Referencias UI")]
    public TextMeshProUGUI notesText;
    public TextMeshProUGUI doorMessage;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCounter()
    {
        notesText.text = $"{NoteProgress.Instance.notesFound}/{NoteProgress.Instance.totalNotes}";
    }

    public void ShowDoorUnlockedMessage()
    {
        doorMessage.text = "Algo se ha abierto...";
        doorMessage.gameObject.SetActive(true);

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 6f);
    }

    private void HideMessage()
    {
        doorMessage.gameObject.SetActive(false);
    }
}
