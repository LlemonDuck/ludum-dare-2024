using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TilemapVisualizer : MonoBehaviour {
    [SerializeField]
    public Tilemap floorTilemap, wallTileMap;
    [SerializeField]
    private TileBase defaultFloorTile, wallNorth, wallEast, wallWest, wallSouth, wallFull, wallCornerNorthEastOpen,
            wallCornerNorthWestOpen, wallCornerSouthEastOpen, wallCornerSouthWestOpen,
            wallCornerSouthEast, wallCornerSouthWest, wallCornerNorthEast, wallCornerNorthWest;
    [SerializeField]
    private List<TileBase> variantFloorTiles;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions) {
        foreach (var position in floorPositions) {
        TileBase tile = defaultFloorTile;
            if (Random.Range(0, 21) > 19) {
                tile = variantFloorTiles[Random.Range(0, variantFloorTiles.Count)];
            }
            PaintSingleTile(floorTilemap, tile, position);
        }
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType) {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTileHelper.wallNorth.Contains(typeAsInt)) {
            tile = wallNorth;
        } else if (WallTileHelper.wallEast.Contains(typeAsInt)) {
            tile = wallEast;
        } else if (WallTileHelper.wallWest.Contains(typeAsInt)) {
            tile = wallWest;
        } else if (WallTileHelper.wallSouth.Contains(typeAsInt)) {
            tile = wallSouth;
        } else if (WallTileHelper.wallFull.Contains(typeAsInt)) {
            tile = wallFull;
        }

        if (tile != null) { 
            PaintSingleTile(wallTileMap, tile, position);
        }
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType) {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTileHelper.IsCornerNorthEastOpen(typeAsInt)) {
            tile = wallCornerNorthEastOpen;
        } else if (WallTileHelper.IsCornerNorthWestOpen(typeAsInt)) {
            tile = wallCornerNorthWestOpen;
        } else if (WallTileHelper.IsCornerSouthEastOpen(typeAsInt)) {
            tile = wallCornerSouthEastOpen;
        } else if (WallTileHelper.IsCornerSouthWestOpen(typeAsInt)) {
            tile = wallCornerSouthWestOpen;
        } else if (WallTileHelper.IsCornerSouthWest(typeAsInt)) {
            tile = wallCornerSouthWest;
        } else if (WallTileHelper.IsCornerSouthEast(typeAsInt)) {
            tile = wallCornerSouthEast;
        } else if (WallTileHelper.IsCornerNorthEast(typeAsInt)) {
            tile = wallCornerNorthEast;
        } else if (WallTileHelper.IsCornerNorthWest(typeAsInt)) {
            tile = wallCornerNorthWest;
        } else if (WallTileHelper.wallFullEightDirections.Contains(typeAsInt)) {
            tile = wallFull;
        } else if (WallTileHelper.wallSouthEightDirections.Contains(typeAsInt)) {
            tile = wallSouth;
        }

        if (tile != null) {
            PaintSingleTile(wallTileMap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear() {
        floorTilemap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
    }
}
