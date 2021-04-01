using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public struct ValidArea {
    public int maxX_left;
    public int maxX_right;
    public int maxY_top;
    public int maxY_bottom;

    public void adjustArea(Vector2 _mapSize, Vector2 _center) {
        if (_center.x == 0 && _center.y == 0) {
            maxX_left = 0;
            maxY_bottom = 0;
            maxX_right = (int)_mapSize.x;
            maxY_top = (int)_mapSize.y;
            return;
        }

        maxY_top = (int)(_mapSize.y - _center.y - 1);
        maxY_bottom = (int)(_center.y - _mapSize.y);
        
        
    }
    
}

public class MapLoader : MonoBehaviour {

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Vector2 tileCenter;
    [SerializeField] private Image defaultTexture;

    [SerializeField] private Transform mapParent;
    
    public List<Tile> tilePrefabList = new List<Tile>();
    public ValidArea validArea = new ValidArea();
    
    void Awake() {
        var _center = new Vector2(mapSize.x / 2, mapSize.y / 2);
        if (_center.x % 2 != 0)
            _center.x--;

        if (_center.y % 2 != 0)
            _center.y--;
        
        TileCalcs.tileCenter = _center;
        validArea.adjustArea(mapSize, tileCenter);
        
        for (var _y = 0; _y < mapSize.y; _y++) {
            for (var _x = 0; _x < mapSize.x; _x++) {
                var _tilePos = TileCalcs.toGrid(_x, _y);

                var _go = Instantiate(tilePrefab, new Vector3(_tilePos.x, _tilePos.y), Quaternion.identity, mapParent);
                _go.GetComponent<SpriteRenderer>().sortingOrder = _y * (int)mapSize.x + _x;

                var _tile = _go.GetComponent<Tile>();
                var _rect = _go.GetComponent<RectTransform>().localPosition;
                var _cellPos = new Vector2(_rect.x / (100 * TileCalcs.tileWidth), _rect.y / (100 * TileCalcs.tileHeight));
                _tile.gridPosition = TileCalcs.getRealCell(_cellPos, _rect, Vector2.zero);
                _tile.size = new Vector2(TileCalcs.tileWidth, TileCalcs.tileHeight);
                
                tilePrefabList.Add(_tile);       
            }
        }
    }

    public void onReload() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
        Awake();
    }

}
