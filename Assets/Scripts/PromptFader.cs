using UnityEngine;

public class PromptFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeSpeed = 6f;

    private bool shouldShow = false;

    void Update()
    {
        float target = shouldShow ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, target, Time.deltaTime * fadeSpeed);
    }

    public void Show() => shouldShow = true;
    public void Hide() => shouldShow = false;
}
