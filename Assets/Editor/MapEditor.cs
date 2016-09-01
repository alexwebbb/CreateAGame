using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        MapGenerator map = (MapGenerator)target;
        map.GenerateMap();
    }
}
