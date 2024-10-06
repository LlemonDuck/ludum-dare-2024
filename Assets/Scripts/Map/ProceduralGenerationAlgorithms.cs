using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGenerationAlgorithms {
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLen) {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLen; i++) {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength) {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var currentPosition = startPosition;
        int directionIndex = Random.Range(0, Direction2D.cardinalDirections.Count);
        var direction = Direction2D.cardinalDirections[directionIndex];
        var perpendicularDirection90 = Direction2D.cardinalDirections[(directionIndex + 1) % Direction2D.cardinalDirections.Count];
        var perpendicularDirection270 = Direction2D.cardinalDirections[Mod(directionIndex - 1, Direction2D.cardinalDirections.Count)];

        int i = 0;
        do {
            corridor.Add(currentPosition);
            corridor.Add(currentPosition + perpendicularDirection90);
            corridor.Add(currentPosition + perpendicularDirection270);
            currentPosition += direction;
            i++;
        } while (i < corridorLength);
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight) {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while(roomsQueue.Count > 0) {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >= minWidth) {
                if(Random.value < 0.5f) {
                    if(room.size.y >= minHeight * 2) {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    } else if (room.size.x >= minWidth * 2) {
                        SplitVertically(minWidth, roomsQueue, room);
                    } else if (room.size.y >= minHeight && room.size.x >= minWidth) {
                        roomsList.Add(room);
                    }
                } else {
                    if (room.size.x >= minWidth * 2) {
                        SplitVertically(minWidth, roomsQueue, room);
                    } else if(room.size.y >= minHeight * 2) {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    } else if (room.size.y >= minHeight && room.size.x >= minWidth) {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room) {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(
            new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z)
        );
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room) {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(
            new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z)
        );
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static int Mod(int x, int m) {
        return (x%m + m)%m;
    }
}


public static class Direction2D {
    public static List<Vector2Int> cardinalDirections = new List<Vector2Int> {
        new Vector2Int(0, 1), // up
        new Vector2Int(1, 0), // right
        new Vector2Int(0, -1), // down
        new Vector2Int(-1, 0), // left
    };

    public static List<Vector2Int> diagonalDirections = new List<Vector2Int> {
        new Vector2Int(1,1), // up - right
        new Vector2Int(1,-1), // right - down
        new Vector2Int(-1, -1), // down - left
        new Vector2Int(-1, 1) // left - up
    };

    public static List<Vector2Int> eightDirections = new List<Vector2Int> {
        new Vector2Int(0,1), // up
        new Vector2Int(1,1), // up-right
        new Vector2Int(1,0), // right
        new Vector2Int(1,-1), // right-down
        new Vector2Int(0, -1), // down
        new Vector2Int(-1, -1), // down-left
        new Vector2Int(-1, 0), // left
        new Vector2Int(-1, 1) // left-up
    };

    public static Vector2Int GetRandomCardinalDirection() {
        return cardinalDirections[Random.Range(0, cardinalDirections.Count)];
    }
}