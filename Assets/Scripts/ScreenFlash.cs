using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    private Image image;
    private Color flashColor;
    private float alpha;
    private float fadeSpeed = 2.5f;

    void Awake()
    {
        image = GetComponent<Image>();
        image.color = Color.clear;
    }

    public void Flash(Color color, float strength)
    {
        flashColor = color;
        alpha = strength;
        image.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
    }

    void Update()
    {
        if (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            alpha = Mathf.Clamp01(alpha);
            image.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
        }
    }
}