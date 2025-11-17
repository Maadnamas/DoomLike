using UnityEngine;

public abstract class CollectableObject : MonoBehaviour, ICollectable
{
    [Header("Floating & Rotation")]
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float floatAmplitude = 0.25f;
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] AudioClip collectSound;

    Vector3 startPos;

    protected virtual void Start()
    {
        startPos = transform.position;
    }

    protected virtual void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        float y = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        Vector3 p = transform.position;
        p.y = y;
        transform.position = p;
    }

    public abstract void Collect();

    protected virtual void OnTriggerEnter(Collider other)
    {
        TryCollect(other);
    }

    protected virtual void TryCollect(Collider other)
    {

    }

    protected virtual void PlaySound()
    {
        AudioManager.PlaySFX2D(collectSound);
    }
}
