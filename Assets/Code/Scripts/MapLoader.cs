using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour {

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Vector2 tileCenter;
    
    private List<GameObject> tilePrefabList;

    private const float widthOffset = 0.16f, heightOffest = 0.075f;
    
    void Start() {
        tilePrefabList = new List<GameObject>();

        for (var _y = 0; _y < mapSize.y; _y++) {
            for (var _x = 0; _x < mapSize.x; _x++) {
                var _tileX = (_x - _y ) * widthOffset;
                var _tileY = -(_x + _y) * heightOffest;

                _tileX += tileCenter.x * 0.32f;
                _tileY -= tileCenter.y * 0.15f;

                var _tile = Instantiate(tilePrefab, new Vector3(_tileX, _tileY), Quaternion.identity);
                _tile.GetComponent<SpriteRenderer>().sortingOrder = _y * (int)mapSize.x + _x;
                
                tilePrefabList.Add(_tile);       
            }
        }
    }
}
