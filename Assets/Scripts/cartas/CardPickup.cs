using UnityEngine;

public class CardPickup : MonoBehaviour, ICollectable
{
    public CardData cardToGive;
    public Renderer cardRenderer;
    public Renderer backrender;
    public float rotationSpeed = 60f;

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
            Material backmat = backrender.material;

            uniqueMaterial.SetTexture("_Base_Texture", cardToGive.cardTexture);
            if (cardToGive.isFoil)
            {
                uniqueMaterial.SetFloat("_IsFoil", 1);
                backmat.SetFloat("_IsFoil", 1);
            }
            else
            {
                uniqueMaterial.SetFloat("_IsFoil", 0);
                backmat.SetFloat("_IsFoil", 0);
            }
        }
        else
        {
            Debug.LogWarning("CardPickup: Faltan referencias para aplicar textura");
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

            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }
}