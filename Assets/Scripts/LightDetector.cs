using UnityEngine;

public class LightDetector : MonoBehaviour
{
    public float detectionRadius = 8f;          // Distancia a la que la luz afecta
    public LayerMask obstacleMask;              // Capas que bloquean la luz
    public float lightLevel;                    // Nivel final de luz (0-1)

    private Light[] lightsInScene;

    void Start()
    {
        lightsInScene = FindObjectsByType<Light>(FindObjectsSortMode.None);
    }

    void Update()
    {
        
        lightLevel = CalculateLightLevel();
    }

    float CalculateLightLevel()
    {
        float total = 0f;
        int count = 0;

        foreach (Light L in lightsInScene)
        {
            if (L == null) continue;

            if (L.type == LightType.Spot || L.type == LightType.Point)
            {
                float dist = Vector3.Distance(transform.position, L.transform.position);

                if (dist <= detectionRadius && dist <= L.range)
                {
                    // Verificar si la luz NO está bloqueada por paredes
                    Vector3 dir = (L.transform.position - transform.position).normalized;

                    if (!Physics.Raycast(transform.position, dir, dist, obstacleMask))
                    {
                        float intensity = (1f - dist / L.range) * L.intensity;
                        total += intensity;
                        count++;
                    }
                }
            }
        }

        if (count == 0) return 0f;

        return Mathf.Clamp01(total / count / 2f);
    }
}
