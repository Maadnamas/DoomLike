using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingIdleEffect : MonoBehaviour
{
    [Header("Movimiento lateral")]
    public float moveAmplitude = 30f;     // cu�nto se mueve hacia los lados (px)
    public float moveSpeed = 1.5f;        // velocidad del vaiv�n

    [Header("Rotaci�n / inclinaci�n")]
    public float tiltAmplitude = 10f;     // grados m�ximos de inclinaci�n
    public float tiltSpeed = 2f;          // velocidad del vaiv�n de rotaci�n

    [Header("Temblor err�tico suave")]
    public float jitterAmount = 3f;       // cu�nto vibra aleatoriamente
    public float jitterSpeed = 8f;        // qu� tan r�pido cambia el temblor

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

        // Peque�o temblor err�tico para romper la simetr�a
        jitterTimer += Time.deltaTime * jitterSpeed;
        float jitterX = Mathf.PerlinNoise(jitterTimer, 0f) * 2f - 1f;
        float jitterY = Mathf.PerlinNoise(0f, jitterTimer) * 2f - 1f;
        Vector2 jitter = new Vector2(jitterX, jitterY) * jitterAmount;

        // Actualizar posici�n
        rect.anchoredPosition = startPos + new Vector2(xOffset, 0f) + jitter;

        // Rotaci�n/inclinaci�n (pendular)
        float zRot = Mathf.Sin((t + tiltOffset) * tiltSpeed) * tiltAmplitude;
        rect.localRotation = Quaternion.Euler(0f, 0f, zRot);
    }
}