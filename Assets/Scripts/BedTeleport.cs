using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BedTeleport : MonoBehaviour
{
    [Header("Destino del teletransporte")]
    public Vector3 teleportPosition;

    [Header("Fade a negro")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    private bool isTeleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTeleporting && other.CompareTag("Player"))
        {
            StartCoroutine(TeleportSequence(other.transform));
        }
    }

    IEnumerator TeleportSequence(Transform player)
    {
        isTeleporting = true;

        // Fade a negro
        yield return StartCoroutine(Fade(0f, 1f));

        // Teletransporte
        player.position = teleportPosition;

        // Pequeña pausa opcional
        yield return new WaitForSeconds(0.2f);

        // Fade de regreso
        yield return StartCoroutine(Fade(1f, 0f));

        isTeleporting = false;
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(start, end, t / fadeDuration);

            fadePanel.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
    }
}
