using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingIdleEffect : MonoBehaviour
{
    [Header("Lateral Movement")]
    public float moveAmplitude = 30f;
    public float moveSpeed = 1.5f;

    [Header("Rotation / Tilt")]
    public float tiltAmplitude = 10f;
    public float tiltSpeed = 2f;

    [Header("Soft Erratic Jitter")]
    public float jitterAmount = 3f;
    public float jitterSpeed = 8f;

    private RectTransform rect;
    private Vector2 startPos;
    private float moveOffset;
    private float tiltOffset;
    private float jitterTimer;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;

        moveOffset = Random.Range(0f, 100f);
        tiltOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time;

        float xOffset = Mathf.Sin((t + moveOffset) * moveSpeed) * moveAmplitude;

        jitterTimer += Time.deltaTime * jitterSpeed;
        float jitterX = Mathf.PerlinNoise(jitterTimer, 0f) * 2f - 1f;
        float jitterY = Mathf.PerlinNoise(0f, jitterTimer) * 2f - 1f;
        Vector2 jitter = new Vector2(jitterX, jitterY) * jitterAmount;

        rect.anchoredPosition = startPos + new Vector2(xOffset, 0f) + jitter;

        float zRot = Mathf.Sin((t + tiltOffset) * tiltSpeed) * tiltAmplitude;
        rect.localRotation = Quaternion.Euler(0f, 0f, zRot);
    }
}