using UnityEngine;

public class CardPickup : MonoBehaviour, ICollectable
{
    public CardData cardToGive;
    public Renderer cardRenderer;
    public Renderer backRenderer;
    public float rotationSpeed = 60f;

    [Header("Audio")]
    public AudioClip pickupSound;

    void Start()
    {
        ApplyCardTexture();

        if (CardCollectionManager.Instance != null &&
            CardCollectionManager.Instance.HasCard(cardToGive.cardID))
        {
            gameObject.SetActive(false);
        }
    }

    void ApplyCardTexture()
    {
        if (cardRenderer != null && cardToGive != null && cardToGive.cardTexture != null)
        {
            Material uniqueMaterial = cardRenderer.material;
            Material backMat = backRenderer.material;

            uniqueMaterial.SetTexture("_Base_Texture", cardToGive.cardTexture);
            if (cardToGive.isFoil)
            {
                uniqueMaterial.SetFloat("_IsFoil", 1);
                backMat.SetFloat("_IsFoil", 1);
            }
            else
            {
                uniqueMaterial.SetFloat("_IsFoil", 0);
                backMat.SetFloat("_IsFoil", 0);
            }
        }
        else
        {
            Debug.LogWarning("CardPickup: Missing references to apply texture");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    public void Collect()
    {
        if (CardCollectionManager.Instance != null)
        {
            CardCollectionManager.Instance.AddCard(cardToGive);

            EventManager.TriggerEvent(GameEvents.CARD_COLLECTED, new CardEventData
            {
                cardID = cardToGive.cardID,
                isFoil = cardToGive.isFoil
            });

            if (pickupSound != null)
            {
                AudioManager.PlaySFX2D(pickupSound);
            }

            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }
}