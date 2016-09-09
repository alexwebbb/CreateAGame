using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform navMeshMaskPrefab;
    public Vector2 mapSize;
    public Vector2 maxMapSize;

    [Range(0, 1)]
    public float outlinePercent;
    [Range(0, 1)]
    public float obstaclePercent;

    public float tileSize = 1;

    public int seed = 10;

    private Coord mapCenter;

    private float tileHalfWidth = 0.5f;
    private Quaternion newTileQuat = Quaternion.Euler(90,0,0);

    private List<Coord> allTileCoords;
    private Queue<Coord> shuffledTileCoords;

    void Start() {
        tileHalfWidth = tilePrefab.localScale.x / 2;

        GenerateMap();
    }

    public void GenerateMap() {

        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

        string holderName = "Generated Map";
        if (transform.FindChild(holderName)) {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                Vector3 tilePosition = CoordToPosition(x, y);
                // instantiate tile
                Transform newTile = (Transform)Instantiate(tilePrefab, tilePosition, newTileQuat);
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++) {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                // instantiate obstacle
                Transform newObstacle = (Transform)Instantiate(obstaclePrefab,
                    obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
                newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newObstacle.parent = mapHolder;
            } else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        Transform maskLeft = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.left * (maxMapSize.x + mapSize.x) / 4f * tileSize, Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2f, 1, mapSize.y) * tileSize;

        Transform maskRight = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.right * (maxMapSize.x + mapSize.x) / 4f * tileSize, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2f, 1, mapSize.y) * tileSize;

        Transform maskTop = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.forward * (maxMapSize.y + mapSize.y) / 4f * tileSize, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2f) * tileSize;

        Transform maskBottom = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.back * (maxMapSize.y + mapSize.y) / 4f * tileSize, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2f) * tileSize;


        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] _obstacleMap, int _currentObstacleCount) {
        bool[,] mapFlags = new bool[_obstacleMap.GetLength(0), _obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;

        int accessibleTileCount = 1;

        while( queue.Count > 0) {
            Coord tile = queue.Dequeue();

            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if(x == 0 || y == 0) {
                        if(neighborX >= 0 && neighborX < _obstacleMap.GetLength(0) 
                            && neighborY >= 0 && neighborY < _obstacleMap.GetLength(1)) {
                            if(!mapFlags[neighborX, neighborY] && !_obstacleMap[neighborX, neighborY]) {
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - _currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;  
    }

    Vector3 CoordToPosition(int _x, int _y) {
        return new Vector3(-mapSize.x / 2 + tileHalfWidth + _x, 
            0, -mapSize.y / 2 + tileHalfWidth + _y) * tileSize;
    }

    public Coord GetRandomCoord() {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord {
        public int x;
        public int y;

        public Coord(int _x, int _y) {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2) {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2) {
            return !(c1 == c2);
        }
    }
}
