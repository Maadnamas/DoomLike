using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MedkitUIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI medkitCountText;
    [SerializeField] private Image[] medkitIcons;
    [SerializeField] private Color fullColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0.3f);

    [Header("Referencias Player")]
    [SerializeField] private PlayerHealth playerHealth;

    private bool isSubscribed = false;

    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        TrySubscribe();
        UpdateMedkitDisplayFromPlayer();
    }

    void OnEnable()
    {
        TrySubscribe();
    }

    void OnDisable()
    {
        if (isSubscribed)
        {
            EventManager.StopListening(GameEvents.UI_UPDATE_MEDKITS, OnMedkitUpdate);
            isSubscribed = false;
        }
    }

    void TrySubscribe()
    {
        if (!isSubscribed && EventManager.Instance != null)
        {
            EventManager.StartListening(GameEvents.UI_UPDATE_MEDKITS, OnMedkitUpdate);
            isSubscribed = true;
        }
    }

    void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribe();
        }

        if (Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.J))
        {
            UpdateMedkitDisplayFromPlayer();
        }
    }

    private void UpdateMedkitDisplayFromPlayer()
    {
        if (playerHealth != null)
        {
            int current = playerHealth.GetMedkitCount();
            int max = playerHealth.GetMaxMedkits();
            UpdateMedkitDisplay(current, max);
        }
    }

    private void OnMedkitUpdate(object data)
    {
        if (data is MedkitEventData medkitData)
        {
            UpdateMedkitDisplay(medkitData.currentMedkits, medkitData.maxMedkits);
        }
    }

    private void UpdateMedkitDisplay(int current, int max)
    {
        if (medkitCountText != null)
        {
            medkitCountText.text = $"{current}/{max}";
        }

        if (medkitIcons != null && medkitIcons.Length > 0)
        {
            for (int i = 0; i < medkitIcons.Length; i++)
            {
                if (medkitIcons[i] != null)
                {
                    medkitIcons[i].color = (i < current) ? fullColor : emptyColor;
                }
            }
        }
    }
}