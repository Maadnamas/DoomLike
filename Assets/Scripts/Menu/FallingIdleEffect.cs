using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingIdleEffect : MonoBehaviour
{
    [Header("Movimiento lateral")]
    public float moveAmplitude = 30f;     // cuánto se mueve hacia los lados (px)
    public float moveSpeed = 1.5f;        // velocidad del vaivén

    [Header("Rotación / inclinación")]
    public float tiltAmplitude = 10f;     // grados máximos de inclinación
    public float tiltSpeed = 2f;          // velocidad del vaivén de rotación

    [Header("Temblor errático suave")]
    public float jitterAmount = 3f;       // cuánto vibra aleatoriamente
    public float jitterSpeed = 8f;        // qué tan rápido cambia el temblor

    private RectTransform rect;
    private Vector2 startPos;
    private float moveOffset;
    private float tiltOffset;
    private float jitterTimer;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;

        moveOffset = Random.Range(0f, 100f); // desfasar cada instancia si hay varias
        tiltOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time;

        // Movimiento suave hacia los costados (como flotando)
        float xOffset = Mathf.Sin((t + moveOffset) * moveSpeed) * moveAmplitude;

        // Pequeño temblor errático para romper la simetría
        jitterTimer += Time.deltaTime * jitterSpeed;
        float jitterX = Mathf.PerlinNoise(jitterTimer, 0f) * 2f - 1f;
        float jitterY = Mathf.PerlinNoise(0f, jitterTimer) * 2f - 1f;
        Vector2 jitter = new Vector2(jitterX, jitterY) * jitterAmount;

        // Actualizar posición
        rect.anchoredPosition = startPos + new Vector2(xOffset, 0f) + jitter;

        // Rotación/inclinación (pendular)
        float zRot = Mathf.Sin((t + tiltOffset) * tiltSpeed) * tiltAmplitude;
        rect.localRotation = Quaternion.Euler(0f, 0f, zRot);
    }
}