using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UnderwaterTrigger : MonoBehaviour
{
    public UniversalRendererData rendererData;
    public string featureName = "UnderwaterFeature";

    ScriptableRendererFeature underwaterFeature;

    void Awake()
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == featureName)
            {
                underwaterFeature = feature;
                break;
            }
        }

        if (underwaterFeature != null)
            underwaterFeature.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && underwaterFeature != null)
        {
            underwaterFeature.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && underwaterFeature != null)
        {
            underwaterFeature.SetActive(false);
        }
    }
}
