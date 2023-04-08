using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneAssets : MonoBehaviour
{
    private static SceneAssets instance;

    public static SceneAssets i
    {
        get
        {
            if (!instance)
            {
                instance = (Resources.Load("SceneAssets") as GameObject).GetComponent<SceneAssets>();
            }
            return instance;
        }
    }

    public Grid grid;
    [Header("Tilemaps")]
    public Tilemap groundTilemap;
    public Tilemap buildingsTilemap;
    public Tilemap waterTilemap;
    public Tilemap selectionTilemap;
}

