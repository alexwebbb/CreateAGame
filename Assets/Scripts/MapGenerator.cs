using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outlinePercent;
    [Range(0, 40)]
    public int obstacleCount = 10;
    public int seed = 10;

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

        for(int i = 0; i < obstacleCount; i++ ) {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
            Transform newObstacle = (Transform)Instantiate(obstaclePrefab, 
                obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
            newObstacle.parent = mapHolder;
        }
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
    }
}
