using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator {
    [SerializeField]
    private int corridorLength = 14, corridorCount = 5;
    [SerializeField]
    [Range(0.1f,1)]
    private float roomPercent = 0.8f;

    protected override void RunProceduralGeneration() {
        CorridorFirstDungeonGeneration();
    }

    private void CorridorFirstDungeonGeneration() {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions, potentialRoomPositions);
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        CreateRoomsAtDeadEnds(FindAllDeadEnds(floorPositions), roomPositions);

        floorPositions.UnionWith(roomPositions);

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions) {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (int i = 0; i < corridorCount; i++) {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions) {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();
        foreach(var roomPosition in roomsToCreate) {
            var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions) {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach(var position in floorPositions) {
            int neighbourCount = 0;
            foreach (var direction in Direction2D.cardinalDirections) {
                if(floorPositions.Contains(position + direction)) {
                    neighbourCount++;
                }
            }
            if(neighbourCount == 1) {
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEndPositions, HashSet<Vector2Int> roomPositions) {
        foreach(var position in deadEndPositions) {
            if (roomPositions.Contains(position)) {
                continue;
            }
            var roomFloor = RunRandomWalk(randomWalkParameters, position);
            roomPositions.Union(roomFloor);
        }
    }
}
