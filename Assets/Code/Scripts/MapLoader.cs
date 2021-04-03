using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ValidArea {
    public int maxX_left = 1000;
    public int maxX_right = -1000;
    public int maxY_top = -1000;
    public int maxY_bottom = 1000;

    public void adjustArea(Vector2 _tilePos) {
        if (_tilePos.x < 0 && _tilePos.x < maxX_left) {
            maxX_left = (int)_tilePos.x;
        } else if(_tilePos.x >= 0 && _tilePos.x > maxX_left) {
            maxX_right = (int)_tilePos.x;
        }

        if (_tilePos.y < 0 && _tilePos.y < maxY_bottom) {
            maxY_bottom = (int)_tilePos.y;
        } else if(_tilePos.y >= 0 && _tilePos.y > maxY_bottom) {
            maxY_top = (int)_tilePos.y;
        }
    }

    public bool mouseInsideMap(Vector2 _mouseCell) {
        return _mouseCell.x >= maxX_left && _mouseCell.x <= maxX_right && _mouseCell.y >= maxY_bottom &&
               _mouseCell.y <= maxY_top;
    }
    
    public override string ToString() {
        return $"T: {maxY_top}, B: {maxY_bottom}, L: {maxX_left}, R: {maxX_right}";
    }
    
}

public class MapLoader : MonoBehaviour {

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Transform mapParent;
    
    public List<Tile> tilePrefabList = new List<Tile>();
    public readonly ValidArea validArea = new ValidArea();
    
    void Awake() {

        Application.targetFrameRate = -1;
        
        Debug.Log($"Aspect Ratio: {(float)Screen.height / (float)Screen.width}, w: {Screen.width}, h: {Screen.height}");

        Debug.Log($"Tile Width: {TileCalcs.tileWidth}, Tile height: {TileCalcs.tileHeight}");

        var _center = new Vector2(mapSize.x / 2, mapSize.y / 2);
        if (_center.x % 2 != 0)
            _center.x--;

        if (_center.y % 2 != 0)
            _center.y--;

        TileCalcs.tileCenter = _center;

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
                
                validArea.adjustArea(_tile.gridPosition);
                tilePrefabList.Add(_tile);       
            }
        }
        
        // For different aspect-ratios
        TileCalcs.tileWidth = TileCalcs.tileWidth * Screen.width / 1920f;
        TileCalcs.tileHeight = TileCalcs.tileHeight * Screen.height / 1080f;
    }

    public void onReload() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
        Awake();
    }

}
