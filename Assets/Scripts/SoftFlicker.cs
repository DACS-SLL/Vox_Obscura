using UnityEngine;

public class MixedFlicker : MonoBehaviour
{
    public Light luz;

    [Header("Flicker suave")]
    public float intensidadBase = 1f;
    public float variacionSuave = 0.3f;
    public float velocidadSuave = 4f;

    [Header("Fallos eléctricos")]
    public float probFallo = 0.01f;
    public float intensidadFallo = 0f;
    public float duracionFallo = 0.1f;

    private bool enFallo = false;
    private float timerFallo = 0f;

    void Start()
    {
        if (luz == null)
            luz = GetComponent<Light>();
    }

    void Update()
    {
        if (!enFallo && Random.value < probFallo)
        {
            enFallo = true;
            timerFallo = duracionFallo;
        }

        if (enFallo)
        {
            luz.intensity = intensidadFallo;
            timerFallo -= Time.deltaTime;

            if (timerFallo <= 0)
                enFallo = false;

            return;
        }

        float ruido = Mathf.PerlinNoise(Time.time * velocidadSuave, 0f);
        luz.intensity = intensidadBase + (ruido - 0.5f) * variacionSuave;
    }
}
