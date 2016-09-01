﻿using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float outlinePercent;

    private float tileHalfWidth;
    private Quaternion newTileQuat = Quaternion.Euler(90,0,0);

    void Start() {
        tileHalfWidth = tilePrefab.localScale.x / 2;

        GenerateMap();
    }

    public void GenerateMap() {

        string holderName = "Generated Map";
        if (transform.FindChild(holderName)) {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x = 0; x < mapSize.x; x++) {
            for(int y = 0; y < mapSize.y; y++) {
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + tileHalfWidth + x,
                    0, -mapSize.y / 2 + tileHalfWidth + y);
                Transform newTile = (Transform)Instantiate(tilePrefab, tilePosition, newTileQuat);
                newTile.localScale = new Vector3(1, 1, 1) * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }
    }
}
