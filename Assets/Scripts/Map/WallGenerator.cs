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
        return GetRectsFromPositions(basicWallPositions);
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

    private static List<Rect> GetRectsFromPositions(HashSet<Vector2Int> positions) {
        List<Rect> positionRects = new List<Rect>(positions.Count / 5);

        foreach (var position in positions) {
            if (positionRects.FindIndex(collider => {
                if ((int)collider.x == position.x) {
                    var resultY = (int)Mathf.Clamp(position.y, collider.y, collider.y + collider.height);
                    if (resultY == position.y) {
                        return true;
                    }
                } else if ((int)collider.y == position.y) {
                    var resultX = (int)Mathf.Clamp(position.x, collider.x, collider.x + collider.width);
                    if (resultX == position.x) {
                        return true;
                    }
                }
                return false;
            }) != -1) {
                continue;
            }

            Vector2Int searchDirection = Vector2Int.zero;

            foreach (var direction in Direction2D.cardinalDirections) {
                bool hasValue = positions.TryGetValue(position + direction, out Vector2Int adjacentWall);
                if (hasValue) {
                    searchDirection = direction;
                    break;
                }
            }

            Vector2Int minWall = position;
            Vector2Int maxWall = position;

            bool hasNewMin = true;
            bool hasNewMax = true;
            do {
                hasNewMin = positions.TryGetValue(minWall - searchDirection, out Vector2Int newMinWall);
                hasNewMax = positions.TryGetValue(maxWall + searchDirection, out Vector2Int newMaxWall);

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
            positionRects.Add(new Rect(x, y, w, h));
        }

        return positionRects;
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
