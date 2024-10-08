using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator: MonoBehaviour {
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    private WallColliderGenerator wallColliderGenerator = null;
    [SerializeField]
    private GameObject workerAntPrefab = null;
    [SerializeField]
    private GameObject woodPrefab = null;
    [SerializeField]
    private GameObject fireAntPrefab = null;
    [SerializeField]
    private GameObject carpenterAntPrefab = null;
    [SerializeField]
    private GameObject queenAntPrefab = null;
    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;

    public static Vector3Int playerSpawnPosition = Vector3Int.zero;
    private List<Vector3Int> workerAntSpawnPositions = new();
    private List<Vector3Int> woodSpawnPositions = new();
    private Vector3Int carpenterAntSpawnPosition = Vector3Int.zero;
    private Vector3Int fireAntSpawnPosition = Vector3Int.zero;
    private Vector3Int queenAntSpawnPosition = Vector3Int.zero;

    protected void Awake() {
        Time.timeScale = 0.0f;
        GenerateDungeon();
        SpawnEntities();
        Time.timeScale = 1.0f;
    }

    public void GenerateDungeon() {
        tilemapVisualizer.Clear();
        wallColliderGenerator.Clear();

        var roomList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt(Vector3Int.zero, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
            minRoomWidth,
            minRoomHeight
        );

        HashSet<Vector2Int> floor = CreateSimpleRooms(roomList);
        floor.UnionWith(CreateRandomRooms(roomList));

        List<Vector2Int> roomCenterPoints = new();
        foreach(var room in roomList) {
            roomCenterPoints.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }
        SetSpawnPoints(roomCenterPoints, floor);
        HashSet<Vector2Int> corridors = ConnectRooms(roomCenterPoints);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        List<Rect> wallColliders = WallGenerator.CreateWalls(floor, tilemapVisualizer);
        wallColliderGenerator.CreateColliders(wallColliders);
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomList) {
        HashSet<Vector2Int> floor = new();
        foreach (var room in roomList) {
            for (int col = offset; col < room.size.x - offset; col++) {
                for (int row = offset; row < room.size.y - offset; row++) {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateRandomRooms(List<BoundsInt> roomList) {
        HashSet<Vector2Int> floor = new();
        for(int i = 0; i < roomList.Count; i++) {
            var roomBounds = roomList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor) {
                // apply offset
                if(position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                        position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset)) {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position) {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new();
        for (int i = 0; i < parameters.iterations; i++) {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLen);
            floorPositions.UnionWith(path);
            if (parameters.shouldRandomizeStartPerIteration) {
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenterPoints) {
        HashSet<Vector2Int> corridors = new();
        var currentRoomCenter = roomCenterPoints[Random.Range(0, roomCenterPoints.Count)];
        roomCenterPoints.Remove(currentRoomCenter);

        while (roomCenterPoints.Count > 0) {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenterPoints);
            roomCenterPoints.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters) {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters) {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance) {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination) {
        HashSet<Vector2Int> corridor = new();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y) {
            if(destination.y > position.y) {
                position += Vector2Int.up;
            } else if(destination.y < position.y) {
                position += Vector2Int.down;
            }
            corridor.Add(position);
            corridor.Add(position + Vector2Int.left);
            corridor.Add(position + Vector2Int.right);
        }
        while (position.x != destination.x) {
            if (destination.x > position.x) {
                position += Vector2Int.right;
            } else if(destination.x < position.x) {
                position += Vector2Int.left;
            }
            corridor.Add(position);
            corridor.Add(position + Vector2Int.up);
            corridor.Add(position + Vector2Int.down);
        }
        return corridor;
    }

    private void SetSpawnPoints(List<Vector2Int> roomCenterPoints, HashSet<Vector2Int> floorPositions) {
        List<Vector2Int> floorPositionsList = new(floorPositions);

        // workers and wood should spawn in random floor positions
        int numCommonSpawns = (int)Math.Sqrt(Math.Sqrt(dungeonHeight * dungeonWidth));
        for (int i = 0; i < numCommonSpawns; i++) {
            workerAntSpawnPositions.Add((Vector3Int)floorPositionsList[Random.Range(0, floorPositions.Count)]);
            woodSpawnPositions.Add((Vector3Int)floorPositionsList[Random.Range(0, floorPositions.Count)]);
        }

        playerSpawnPosition = (Vector3Int)roomCenterPoints[0];
        carpenterAntSpawnPosition = (Vector3Int)floorPositionsList[1];
        fireAntSpawnPosition = (Vector3Int)floorPositionsList[^2];
        queenAntSpawnPosition = (Vector3Int)roomCenterPoints[^1];
    }

    private void SpawnEntities() {
        foreach(Vector3Int position in workerAntSpawnPositions) {
            SpawnEntityAtPosition(workerAntPrefab, position);
        }
        foreach(Vector3Int position in woodSpawnPositions) {
            SpawnEntityAtPosition(woodPrefab, position);
        }
        SpawnEntityAtPosition(carpenterAntPrefab, carpenterAntSpawnPosition);
        SpawnEntityAtPosition(fireAntPrefab, fireAntSpawnPosition);
        SpawnEntityAtPosition(queenAntPrefab, queenAntSpawnPosition);
    }

    private void SpawnEntityAtPosition(GameObject entityPrefab, Vector3Int position) {
        GameObject entity = Instantiate(entityPrefab);
        entity.transform.position = position;
    }
}
