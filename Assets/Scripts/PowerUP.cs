using UnityEngine;

public class PowerUP : CollectableObject
{
    [Header("Power Settings")]
    [SerializeField] float duration = 5f;

    public override void Collect()
    {
        Destroy(gameObject);
    }

    protected override void TryCollect(Collider other)
    {
        PlayerHealth p = other.GetComponentInChildren<PlayerHealth>();

        if (p != null)
        {
            p.ActivatePowerUp(duration);
            Collect();
        }
    }
}
