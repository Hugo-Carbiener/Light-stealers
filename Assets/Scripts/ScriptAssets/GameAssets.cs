using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets i
    {
        get
        {
            if (!instance) {
                instance = (Resources.Load("GameAssets") as GameObject).GetComponent<GameAssets>();
            } 
            return instance;
        }
    }

    [Header("Tiles")]
    public Tile plainTile;
    public Tile forestTile;
    public Tile mountainTile;
    public Tile waterTile;
    public Tile cityTile;
    public List<Tile> plainTiles;
    public List<Tile> forestTiles;
    public List<Tile> mountainTiles;
    public List<Tile> waterTiles;
    public List<Tile> buildingTiles;
    public TileBase selectionTile;
    public Tile baseForestTile;
    public Tile basePlainTile;
    public Tile baseMountainTile;

    [Header("Sprites")]
    [Header("Ground Sprites")]
    public Sprite plainTileSprite;
    public Sprite forestTileSprite;
    public Sprite mountainTileSprite;
    [Header("Resources Sprites")]
    public Sprite foodIcon;
    public Sprite woodIcon;
    public Sprite stoneIcon;
    [Header("Building Sprites")]
    public Sprite sawmill;
    public Sprite windmill;

}

