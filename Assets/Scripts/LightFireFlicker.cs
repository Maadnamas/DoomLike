using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFireHybridFlicker : MonoBehaviour
{
    [Header("Intensity Range")]
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;

    [Header("PingPong Motion")]
    [SerializeField] private float pingPongSpeed = 2f;

    [Header("Random Flicker")]
    [SerializeField] private float randomStrength = 0.1f;
    [SerializeField] private float noiseSpeed = 2f;

    private Light pointLight;
    private float noiseOffset;

    private void Awake()
    {
        pointLight = GetComponent<Light>();
        noiseOffset = Random.Range(0f, 999f);
    }

    private void Update()
    {
        // --- Ping Pong base ---
        float pingPong = Mathf.PingPong(Time.time * pingPongSpeed, 1f);
        float baseIntensity = Mathf.Lerp(minIntensity, maxIntensity, pingPong);

        // --- Smooth Perlin Noise ---
        float noise = (Mathf.PerlinNoise(Time.time * noiseSpeed, noiseOffset) - 0.5f) * randomStrength;

        // --- Final Result ---
        pointLight.intensity = baseIntensity + noise;
    }
}