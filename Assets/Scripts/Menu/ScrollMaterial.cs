using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMaterial : MonoBehaviour
{
    public Renderer targetRenderer;   // el Renderer del objeto (MeshRenderer)
    public float scrollSpeedY = 0.1f; // velocidad vertical (positiva: sube, negativa: baja)
    public float scrollSpeedX = 0f;   // opcional, si querés movimiento lateral

    private Material mat;
    private Vector2 offset;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        // Importante: instanciar el material para no modificar el material global
        mat = targetRenderer.material;
        offset = mat.mainTextureOffset;
    }

    void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        mat.mainTextureOffset = offset;
    }
}