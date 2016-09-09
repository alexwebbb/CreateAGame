using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform navMeshMaskPrefab;
    public Vector2 maxMapSize;

    [Range(0, 1)]
    public float outlinePercent;

    public float tileSize = 1;

    private float tileHalfWidth = 0.5f;
    private Quaternion newTileQuat = Quaternion.Euler(90,0,0);

    private List<Coord> allTileCoords;
    private Queue<Coord> shuffledTileCoords;

    private Map currentMap;

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {
        currentMap = maps[mapIndex];
        System.Random pRNG = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 
            0.5f, currentMap.mapSize.y * tileSize);

        // Generating coords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++) {
            for (int y = 0; y < currentMap.mapSize.y; y++) {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // Create map holder object
        string holderName = "Generated Map";
        if (transform.FindChild(holderName)) {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++) {
            for (int y = 0; y < currentMap.mapSize.y; y++) {
                Vector3 tilePosition = CoordToPosition(x, y);
                // instantiate tile
                Transform newTile = (Transform)Instantiate(tilePrefab, tilePosition, newTileQuat);
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        // Spawning obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercentage);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++) {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCenter 
                && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {

                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, 
                    currentMap.maxObstacleHeight, (float)pRNG.NextDouble());

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                // instantiate obstacle
                Transform newObstacle = (Transform)Instantiate(obstaclePrefab,
                    obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity);

                // scale object, implement obstacle height
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, 
                    obstacleHeight, (1 - outlinePercent) * tileSize);
                newObstacle.parent = mapHolder;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

            } else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        // Creating navMesh mask
        Transform maskLeft = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.left * (maxMapSize.x + currentMap.mapSize.x) / 4f * tileSize, Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.right * (maxMapSize.x + currentMap.mapSize.x) / 4f * tileSize, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.forward * (maxMapSize.y + currentMap.mapSize.y) / 4f * tileSize, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = (Transform)Instantiate(navMeshMaskPrefab,
            Vector3.back * (maxMapSize.y + currentMap.mapSize.y) / 4f * tileSize, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;


        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] _obstacleMap, int _currentObstacleCount) {
        bool[,] mapFlags = new bool[_obstacleMap.GetLength(0), _obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

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

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - _currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;  
    }

    Vector3 CoordToPosition(int _x, int _y) {
        return new Vector3(-currentMap.mapSize.x /  2f + tileHalfWidth + _x, 
            0, -currentMap.mapSize.y /  2f + tileHalfWidth + _y) * tileSize;
    }

    public Coord GetRandomCoord() {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
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

    [System.Serializable]
    public class Map {

        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercentage;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCenter {
            get {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}
