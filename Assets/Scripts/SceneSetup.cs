using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneSetup : MonoBehaviour
{
    [SerializeField] private Color levelFogColor = new Color(166,68,68);
    [SerializeField] private Material fog;
    void OnEnable()
    {
        fog.SetColor("_colorniebla", levelFogColor);
    }

}
