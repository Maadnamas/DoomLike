using UnityEngine;
using UnityEngine.UI;

public class CloudScroll : MonoBehaviour
{
    public RawImage rawImage;
    public float scrollSpeed = 0.2f; // velocidad hacia arriba

    private Rect uvRect;

    void Start()
    {
        if (rawImage == null)
            rawImage = GetComponent<RawImage>();

        uvRect = rawImage.uvRect;
    }

    void Update()
    {
        uvRect.y += scrollSpeed * Time.deltaTime; // movimiento hacia arriba
        rawImage.uvRect = uvRect;
    }
}