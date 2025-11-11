using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CardPickup : MonoBehaviour
{
    public CardData cardToGive;

    [Header("Rotación")]
    [Tooltip("Velocidad de rotación en grados por segundo.")]
    public float rotationSpeed = 60f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (cardToGive != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = cardToGive.image;
        }

        if (CardCollectionManager.Instance != null &&
            CardCollectionManager.Instance.HasCard(cardToGive.cardID))
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CardCollectionManager.Instance.AddCard(cardToGive);
            gameObject.SetActive(false);
        }
    }
}
