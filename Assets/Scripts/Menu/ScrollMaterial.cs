using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMaterial : MonoBehaviour
{
    public Renderer targetRenderer;
    public float scrollSpeedY = 0.1f;
    public float scrollSpeedX = 0f;

    private Material mat;
    private Vector2 offset;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

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