using UnityEngine;

[ExecuteInEditMode]
public class SceneSetup : MonoBehaviour
{
    public static SceneSetup Instance { get; private set; }

    [SerializeField] private Color levelFogColor = new Color(166, 68, 68);
    [SerializeField] private Material fog;

    [field: SerializeField] public int RankSThreshold { get; private set; } = 2500;
    [field: SerializeField] public int RankAThreshold { get; private set; } = 1800;
    [field: SerializeField] public int RankBThreshold { get; private set; } = 1000;
    [field: SerializeField] public int RankCThreshold { get; private set; } = 500;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        if (Application.isPlaying && fog != null)
        {
            fog.SetColor("_colorniebla", levelFogColor);
        }
    }
}