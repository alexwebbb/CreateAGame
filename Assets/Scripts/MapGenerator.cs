using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outlinePercent;
    [Range(0, 1)]
    public float obstaclePercent;
    public int seed = 10;

    private Coord mapCenter;

    private float tileHalfWidth;
    private Quaternion newTileQuat = Quaternion.Euler(90,0,0);

    private List<Coord> allTileCoords;
    private Queue<Coord> shuffledTileCoords;

    void Start() {
        tileHalfWidth = tilePrefab.localScale.x / 2;

        GenerateMap();
    }

    public void GenerateMap() {

        allTileCoords = new List<Coord>();
        for(int x = 0; x < mapSize.x; x++) {
            for(int y = 0; y < mapSize.y; y++) {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray (allTileCoords.ToArray(), seed));
        mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

        string holderName = "Generated Map";
        if (transform.FindChild(holderName)) {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x = 0; x < mapSize.x; x++) {
            for(int y = 0; y < mapSize.y; y++) {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = (Transform)Instantiate(tilePrefab, tilePosition, newTileQuat);
                newTile.localScale = new Vector3(1, 1, 1) * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x,(int)mapSize.y];
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;

        for(int i = 0; i < obstacleCount; i++ ) {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = (Transform)Instantiate(obstaclePrefab,
                    obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
                newObstacle.parent = mapHolder; 
            } else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            } 
        }
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
            0, -mapSize.y / 2 + tileHalfWidth + _y);
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
