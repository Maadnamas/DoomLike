using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(LarvaMedkit))]
public class LarvaSlimeTrail : MonoBehaviour
{
    [Header("Configuración de Baba")]
    [SerializeField] private GameObject slimeDecalPrefab;
    [SerializeField] private Material slimeMaterial;
    [SerializeField] private float decalSize = 0.5f;
    [SerializeField] private float dropInterval = 1.5f;
    [SerializeField] private float decalHeight = 0.01f;
    [SerializeField] private float slimeLifetime = 5f;

    [Header("Fade Out")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private bool useDissolveEffect = true;
    //[SerializeField] private float dissolveSpeed = 0.2f;

    [Header("Raycast")]
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxSlopeAngle = 45f;

    private LarvaMedkit larva;
    private float lastDropTime = 0f;
    private List<GameObject> activeSlimes = new List<GameObject>();

    void Start()
    {
        larva = GetComponent<LarvaMedkit>();

        // Crear el prefab si no existe
        if (slimeDecalPrefab == null)
        {
            CreateSlimePrefab();
        }
    }

    void Update()
    {
        if (larva == null || larva.enabled == false) return;

        // Dejar baba en intervalos regulares
        if (Time.time - lastDropTime >= dropInterval)
        {
            DropSlimeUnderLarva();
            lastDropTime = Time.time;
        }

        // Limpiar babas antiguas
        CleanupOldSlimes();
    }

    void DropSlimeUnderLarva()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;

        // Raycast hacia abajo para encontrar el suelo
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            // Verificar ángulo de pendiente
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (slopeAngle > maxSlopeAngle) return;

            // Crear la baba
            CreateSlimeAtPosition(hit.point, hit.normal);
        }
    }

    void CreateSlimeAtPosition(Vector3 position, Vector3 normal)
    {
        if (slimeDecalPrefab == null) return;

        // Instanciar el decal
        GameObject slime = Instantiate(slimeDecalPrefab);
        slime.transform.position = position + normal * decalHeight;
        slime.transform.rotation = Quaternion.LookRotation(-normal);

        // Configurar tamaño
        DecalProjector decal = slime.GetComponent<DecalProjector>();
        if (decal != null)
        {
            decal.size = new Vector3(decalSize, decalSize, 0.1f);

            // Crear nuevo material instance para poder modificar las propiedades
            if (slimeMaterial != null)
            {
                Material matInstance = new Material(slimeMaterial);
                decal.material = matInstance;

                // Iniciar fade out
                StartCoroutine(FadeOutSlime(slime, matInstance));
            }
        }

        activeSlimes.Add(slime);

        // Destruir después del tiempo de vida
        Destroy(slime, slimeLifetime + 1f);
    }

    System.Collections.IEnumerator FadeOutSlime(GameObject slime, Material material)
    {
        float elapsedTime = 0f;
        DecalProjector decal = slime.GetComponent<DecalProjector>();

        while (elapsedTime < slimeLifetime && slime != null && material != null)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / slimeLifetime);

            if (useDissolveEffect)
            {
                // Efecto de disolución
                if (material.HasProperty("_DissolveAmount"))
                {
                    material.SetFloat("_DissolveAmount", progress);
                }
            }
            else
            {
                // Fade simple de alpha
                if (material.HasProperty("_BaseColor"))
                {
                    Color color = material.GetColor("_BaseColor");
                    color.a = fadeCurve.Evaluate(progress);
                    material.SetColor("_BaseColor", color);
                }
            }

            yield return null;
        }

        // Remover de la lista cuando se destruye
        if (slime != null && activeSlimes.Contains(slime))
        {
            activeSlimes.Remove(slime);
        }
    }

    void CleanupOldSlimes()
    {
        // Remover nulls de la lista
        activeSlimes.RemoveAll(slime => slime == null);

        // Mantener un máximo de babas activas para performance
        if (activeSlimes.Count > 20)
        {
            GameObject oldestSlime = activeSlimes[0];
            if (oldestSlime != null)
            {
                Destroy(oldestSlime);
            }
            activeSlimes.RemoveAt(0);
        }
    }

    void CreateSlimePrefab()
    {
        // Crear un GameObject simple con DecalProjector
        GameObject prefab = new GameObject("SlimeDecalPrefab");

        // Añadir DecalProjector
        DecalProjector decal = prefab.AddComponent<DecalProjector>();
        decal.size = new Vector3(decalSize, decalSize, 0.1f);
        decal.fadeFactor = 1f;

        // Añadir un script simple para auto-destrucción
        prefab.AddComponent<DestroyAfterTime>().lifetime = slimeLifetime;

        // Guardar como prefab (esto lo puedes hacer manualmente en el editor)
        slimeDecalPrefab = prefab;
    }

    void OnDestroy()
    {
        // Limpiar todas las babas cuando la larva es destruida
        foreach (GameObject slime in activeSlimes)
        {
            if (slime != null)
            {
                Destroy(slime);
            }
        }
        activeSlimes.Clear();
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar el raycast
        Gizmos.color = Color.green;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * raycastDistance);
        Gizmos.DrawWireSphere(rayStart, 0.05f);
    }
}
public class DestroyAfterTime : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}