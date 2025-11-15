using UnityEngine;

enum AmmoType
{
    Pistol = 1,
    Rocket = 2,
    Sniper = 3
}

public class AmmoBox : MonoBehaviour, ICollectable
{
    [Header("Ammo Settings")]
    [SerializeField] private AmmoType m_AmmoType;
    [SerializeField] private int m_AmmoCount;

    [Header("Floating & Rotation")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Rotate();
        Float();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void Float()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }

    public void Collect()
    {
        EventManager.TriggerEvent(GameEvents.AMMO_PICKED_UP, new AmmoEventData
        {
            weaponType = m_AmmoType.ToString(),
            amount = m_AmmoCount,
            totalAmmo = m_AmmoCount
        });

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponManager pj = other.gameObject.GetComponentInChildren<WeaponManager>();

        if (pj != null && pj.ReloadAmmo((int)m_AmmoType, m_AmmoCount))
        {
            Collect();
        }
    }
}
