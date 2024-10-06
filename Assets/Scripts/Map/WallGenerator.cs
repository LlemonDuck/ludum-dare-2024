using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class WallGenerator {
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer) {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirections);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirections);
        // TODO: one method for both
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
    }

    private static void CreateBasicWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions) {
        foreach(var position in basicWallPositions) {
            string neighboursBinaryStr = "";
            foreach(var direction in Direction2D.cardinalDirections) {
                var neighbourPosition = position + direction;
                neighboursBinaryStr += floorPositions.Contains(neighbourPosition) ? "1" : "0"; 
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryStr);
        }
    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions) {
        foreach (var position in cornerWallPositions) {
            string neighboursBinaryStr = "";
            foreach (var direction in Direction2D.eightDirections) {
                var neighbourPosition = position + direction;
                neighboursBinaryStr += floorPositions.Contains(neighbourPosition) ? "1" : "0";
            }
            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryStr);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directions) {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach(var position in floorPositions) {
            foreach(var direction in directions) {
                var neighbourPosition = position + direction;
                if (!floorPositions.Contains(neighbourPosition)) {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }
        return wallPositions;
    }
}
