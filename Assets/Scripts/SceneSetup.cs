using UnityEngine;

[ExecuteInEditMode]
public class SceneSetup : MonoBehaviour
{
    // Estructura serializable para definir un rango
    [System.Serializable]
    public struct RankData
    {
        public string RankName;      // Ej: "S", "A", "B"
        public int RequiredScore;    // Puntaje mínimo para alcanzar este rango
        public Sprite RankSprite;    // Imagen a mostrar en la UI
    }

    public static SceneSetup Instance { get; private set; }

    [Header("Configuración de Niebla")]
    [SerializeField] private Color levelFogColor = new Color(166, 68, 68);
    [SerializeField] private Material fog;

    [Header("Configuración de Ranking por Niveles")]
    [Tooltip("Define los rangos ordenados de MAYOR a MENOR puntaje requerido.")]
    [SerializeField] private RankData[] ranks;

    // Propiedad pública para acceder al array
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