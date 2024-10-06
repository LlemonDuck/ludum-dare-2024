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
    private TileBase defaultFloorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull,  wallInnerCornerDownLeft, wallInnerCornerDownRight,
            wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;
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
        if (WallTileHelper.wallTop.Contains(typeAsInt)) {
            tile = wallTop;
        } else if (WallTileHelper.wallSideRight.Contains(typeAsInt)) {
            tile = wallSideRight;
        } else if (WallTileHelper.wallSideLeft.Contains(typeAsInt)) {
            tile = wallSideLeft;
        } else if (WallTileHelper.wallBottom.Contains(typeAsInt)) {
            tile = wallBottom;
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
        if (WallTileHelper.wallInnerCornerDownLeft.Contains(typeAsInt)) {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTileHelper.wallInnerCornerDownRight.Contains(typeAsInt)) {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTileHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt)) {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTileHelper.wallDiagonalCornerDownRight.Contains(typeAsInt)) {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTileHelper.wallDiagonalCornerUpRight.Contains(typeAsInt)) {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTileHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt)) {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTileHelper.wallFullEightDirections.Contains(typeAsInt)) {
            tile = wallFull;
        }
        else if (WallTileHelper.wallBottmEightDirections.Contains(typeAsInt)) {
            tile = wallBottom;
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
