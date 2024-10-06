using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class WallGenerator {
    public static List<Rect> CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer) {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirections);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirections);
        CreateBasicWalls(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);
        

        List<Rect> wallColliders = new List<Rect>(basicWallPositions.Count / 5);

        foreach (var wall in basicWallPositions) {
            if (wallColliders.FindIndex(collider => {
                if ((int)collider.x == wall.x) {
                    var resultY = (int)Mathf.Clamp(wall.y, collider.y, collider.y + collider.height);
                    if (resultY == wall.y) {
                        return true;
                    }
                } else if ((int)collider.y == wall.y) {
                    var resultX = (int)Mathf.Clamp(wall.x, collider.x, collider.x + collider.width);
                    if (resultX == wall.x) {
                        return true;
                    }
                }
                return false;
            }) != -1) {
                continue;
            }

            Vector2Int searchDirection = Vector2Int.zero;

            foreach (var direction in Direction2D.cardinalDirections) {
                bool hasValue = basicWallPositions.TryGetValue(wall + direction, out Vector2Int adjacentWall);
                if (hasValue) {
                    searchDirection = direction;
                    break;
                }
            }

            Vector2Int minWall = wall;
            Vector2Int maxWall = wall;

            bool hasNewMin = true;
            bool hasNewMax = true;
            do {
                hasNewMin = basicWallPositions.TryGetValue(minWall - searchDirection, out Vector2Int newMinWall);
                hasNewMax = basicWallPositions.TryGetValue(maxWall + searchDirection, out Vector2Int newMaxWall);

                if (hasNewMin) {
                    minWall = newMinWall;
                }

                if (hasNewMax) {
                    maxWall = newMaxWall;
                }
            } while(searchDirection != Vector2Int.zero && (hasNewMin || hasNewMax));

            float x = Mathf.Min(minWall.x, maxWall.x);
            float y = Mathf.Min(minWall.y, maxWall.y);
            float w = Mathf.Abs(minWall.x - maxWall.x);
            float h = Mathf.Abs(minWall.y - maxWall.y);
            wallColliders.Add(new Rect(x, y, w, h));
        }

        return wallColliders;
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
