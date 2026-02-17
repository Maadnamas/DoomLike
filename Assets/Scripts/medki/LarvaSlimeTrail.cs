using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(LarvaMedkit))]
public class LarvaSlimeTrail : MonoBehaviour
{
    [Header("Slime Configuration")]
    [SerializeField] private GameObject slimeDecalPrefab;
    [SerializeField] private Material slimeMaterial;
    [SerializeField] private float decalSize = 0.5f;
    [SerializeField] private float dropInterval = 1.5f;
    [SerializeField] private float decalHeight = 0.01f;
    [SerializeField] private float slimeLifetime = 5f;

    [Header("Fade Out")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private bool useDissolveEffect = true;

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

        if (slimeDecalPrefab == null)
        {
            CreateSlimePrefab();
        }
    }

    void Update()
    {
        if (larva == null || larva.enabled == false) return;

        if (Time.time - lastDropTime >= dropInterval)
        {
            DropSlimeUnderLarva();
            lastDropTime = Time.time;
        }

        CleanupOldSlimes();
    }

    void DropSlimeUnderLarva()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (slopeAngle > maxSlopeAngle) return;

            CreateSlimeAtPosition(hit.point, hit.normal);
        }
    }

    void CreateSlimeAtPosition(Vector3 position, Vector3 normal)
    {
        if (slimeDecalPrefab == null) return;

        GameObject slime = Instantiate(slimeDecalPrefab);
        slime.transform.position = position + normal * decalHeight;
        slime.transform.rotation = Quaternion.LookRotation(-normal);

        DecalProjector decal = slime.GetComponent<DecalProjector>();
        if (decal != null)
        {
            decal.size = new Vector3(decalSize, decalSize, 0.1f);

            if (slimeMaterial != null)
            {
                Material matInstance = new Material(slimeMaterial);
                decal.material = matInstance;

                StartCoroutine(FadeOutSlime(slime, matInstance));
            }
        }

        activeSlimes.Add(slime);

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
                if (material.HasProperty("_DissolveAmount"))
                {
                    material.SetFloat("_DissolveAmount", progress);
                }
            }
            else
            {
                if (material.HasProperty("_BaseColor"))
                {
                    Color color = material.GetColor("_BaseColor");
                    color.a = fadeCurve.Evaluate(progress);
                    material.SetColor("_BaseColor", color);
                }
            }

            yield return null;
        }

        if (slime != null && activeSlimes.Contains(slime))
        {
            activeSlimes.Remove(slime);
        }
    }

    void CleanupOldSlimes()
    {
        activeSlimes.RemoveAll(slime => slime == null);

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
        GameObject prefab = new GameObject("SlimeDecalPrefab");

        DecalProjector decal = prefab.AddComponent<DecalProjector>();
        decal.size = new Vector3(decalSize, decalSize, 0.1f);
        decal.fadeFactor = 1f;

        prefab.AddComponent<DestroyAfterTime>().lifetime = slimeLifetime;

        slimeDecalPrefab = prefab;
    }

    void OnDestroy()
    {
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