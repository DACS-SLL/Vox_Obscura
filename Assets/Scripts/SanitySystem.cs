using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PostPP = UnityEngine.Rendering.PostProcessing;

public class SanitySystem : MonoBehaviour
{
    public LightDetector lightDetector;
    public static SanitySystem Instance;
    public GameObject gameOverScreen;

    [Header("Sanidad según oscuridad")]
    public float sanityDrainInDark = 5f;
    public float sanityDrainInDim = 1f;
    public float lightThresholdDim = 0.4f;
    public float lightThresholdDark = 0.15f;

    [Header("Recuperación de Sanidad")]
    public float sanityRegenRate = 2f;
    public float lightThresholdRegen = 0.2f;

    [Header("Valores")]
    public float maxSanity = 100f;
    public float currentSanity;

    [Header("UI")]
    public Image sanityMask;
    public TextMeshProUGUI sanityText;
    public Image pulsePanel;

    [Header("AudioSources Ambientales")]
    public AudioSource ambienteNormal;
    public AudioSource ambienteDistorsionado;
    public AudioSource murmullosLeves;
    public AudioSource murmullosFuertes;
    public AudioSource soloMurmullos;

    [Header("Audio Clips Puntuales")]
    public AudioClip golpeMetal;
    public AudioClip sonidosTerror;

    [Header("Post-Processing")]
    public PostPP.PostProcessVolume postProcessVolume;
    private PostPP.ChromaticAberration chromAber;
    private PostPP.LensDistortion lensDist;
    private PostPP.Vignette vignette;
    private PostPP.DepthOfField dof;

    [Header("Configuración visual")]
    public float pulseIntensity = 0.2f;

    private AudioSource currentAmbient;

    private bool golpeMetalPlayed = false;
    private bool sonidosTerrorPlayed = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentSanity = maxSanity;
        UpdateUI();
        PlayAmbient(ambienteNormal);

        postProcessVolume.profile.TryGetSettings(out chromAber);
        postProcessVolume.profile.TryGetSettings(out lensDist);
        postProcessVolume.profile.TryGetSettings(out vignette);
        postProcessVolume.profile.TryGetSettings(out dof);
    }

    void Update()
    {
        // Si ya está muerto, no sigue bajando ni subiendo
        if (currentSanity <= 0)
            return;

        float luz = lightDetector.lightLevel;

        if (luz < lightThresholdDark)
        {
            ReduceSanity(sanityDrainInDark * Time.deltaTime);
        }
        else if (luz < lightThresholdDim)
        {
            ReduceSanity(sanityDrainInDim * Time.deltaTime);
        }
        else if (luz >= lightThresholdRegen)
        {
            currentSanity += sanityRegenRate * Time.deltaTime;
            currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
        }

        UpdateUI();
    }

    public void ReduceSanity(float amount)
    {
        currentSanity -= amount;
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);

        if (currentSanity <= 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("GAME OVER: Sanidad llegó a 0");

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
    }

    void UpdateUI()
    {
        float fill = currentSanity / maxSanity;

        sanityMask.fillAmount = fill;
        sanityText.text = $"Sanidad {Mathf.RoundToInt(fill * 100)}%";
        sanityMask.color = GetSanityColor(fill);
        sanityText.color = GetSanityColor(fill);

        HandlePulse(fill);
        HandleAudio(fill);
        HandlePostProcessing(fill);
    }

    Color GetSanityColor(float fill)
    {
        if (fill > 0.75f) return Color.green;
        if (fill > 0.45f) return new Color(1f, 0.85f, 0f);
        if (fill > 0.20f) return new Color(1f, 0.5f, 0f);
        return Color.red;
    }

    void HandlePulse(float fill)
    {
        if (pulsePanel != null)
        {
            float pulse = Mathf.Sin(Time.time * Mathf.Lerp(2f, 8f, 1 - fill)) * (1 - fill) * pulseIntensity;
            pulsePanel.color = new Color(1f, 0f, 0f, pulse);
        }
    }

    void HandleAudio(float fill)
    {
        AudioSource nextAmbient = ambienteNormal;

        if (fill > 0.45f) nextAmbient = ambienteNormal;
        else if (fill > 0.45f && fill > 0.25f) nextAmbient = murmullosLeves;
        else if (fill <= 0.25f && fill > 0.15f) nextAmbient = murmullosFuertes;
        else nextAmbient = soloMurmullos;

        if (nextAmbient != currentAmbient)
            PlayAmbient(nextAmbient);

        if (!golpeMetalPlayed && fill <= 0.25f && fill > 0.15f)
        {
            PlayClipOnce(golpeMetal);
            golpeMetalPlayed = true;
        }

        if (!sonidosTerrorPlayed && fill <= 0.15f)
        {
            PlayClipOnce(sonidosTerror);
            sonidosTerrorPlayed = true;
        }
    }

    void PlayAmbient(AudioSource next)
    {
        if (currentAmbient != null) currentAmbient.Stop();
        currentAmbient = next;
        currentAmbient.loop = true;
        currentAmbient.Play();
    }

    void PlayClipOnce(AudioClip clip)
    {
        if (clip != null && currentAmbient != null)
            currentAmbient.PlayOneShot(clip);
    }

    void HandlePostProcessing(float fill)
    {
        if (chromAber != null) chromAber.intensity.value = Mathf.Lerp(0f, 0.5f, 1 - fill);
        if (lensDist != null) lensDist.intensity.value = Mathf.Lerp(0f, -30f, 1 - fill);
        if (vignette != null) vignette.intensity.value = Mathf.Lerp(0.2f, 0.6f, 1 - fill);
        if (dof != null) dof.aperture.value = Mathf.Lerp(5f, 2f, 1 - fill);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // reanudar juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
