using UnityEngine;

[ExecuteInEditMode]
public class SceneSetup : MonoBehaviour
{
    [System.Serializable]
    public struct RankData
    {
        public string RankName;
        public int RequiredScore;
        public Sprite RankSprite;      // Sprite para la imagen principal (rotación)
        public Sprite LarvaSprite;     // NUEVO: Sprite específico para la imagen de la larva (salto)
        public bool LarvaJumpRank;
    }

    public static SceneSetup Instance { get; private set; }

    [Header("Configuración de Niebla")]
    [SerializeField] private Color levelFogColor = new Color(166, 68, 68);
    [SerializeField] private Material fog;

    [Header("Configuración de Ranking por Niveles")]
    [Tooltip("Define los rangos ordenados de MAYOR a MENOR puntaje requerido.")]
    [SerializeField] private RankData[] ranks;

    public RankData[] Ranks => ranks;

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