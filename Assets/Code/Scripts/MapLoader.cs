using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

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
    
    public List<Cell> tilePrefabList = new List<Cell>();
    public readonly ValidArea validArea = new ValidArea();
    public static Map mapJson;

    private Dictionary<string, GameObject> loadedFromResources = new Dictionary<string, GameObject>();
    
    void Start() {
        Debug.Log($"Aspect Ratio: {(float)Screen.height / (float)Screen.width}, w: {Screen.width}, h: {Screen.height}");
        Debug.Log($"Tile Width: {TileCalcs.tileWidth}, Tile height: {TileCalcs.tileHeight}");

        var _mapUrl = $"{Application.dataPath}/Data/map.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        mapJson = JsonConvert.DeserializeObject<Map>(_json);

        var _center = new Vector2(mapSize.x / 2, mapSize.y / 2);
        if (_center.x % 2 != 0)
            _center.x--;

        if (_center.y % 2 != 0)
            _center.y--;

        TileCalcs.tileCenter = _center;

        for (var _y = 0; _y < mapJson.mapInfo.height; _y++) {
            for (var _x = 0; _x < mapJson.mapInfo.width; _x++) {
                var _tilePos = TileCalcs.toGrid(_x, _y);
                var _spawnOrigin = _tilePos * 100;
                var _orthographicPos = new Vector2(_spawnOrigin.x / (100 * TileCalcs.tileWidth), _spawnOrigin.y / (100 * TileCalcs.tileHeight));
                var _isometricPos = TileCalcs.getRealCell(_orthographicPos, _spawnOrigin, Vector2.zero);
                
                var _go = getTilePrefab((int)_isometricPos.x, (int)_isometricPos.y, _tilePos.x, _tilePos.y);

                var _layer = _y * (int) mapSize.x + _x;
                _go.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = _layer;
                _go.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sortingOrder = _layer;
                _go.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sortingOrder = _layer;
                
                var _tile = _go.GetComponent<Cell>();
                _tile.gridPosition = _isometricPos;
                _tile.size = new Vector2(TileCalcs.tileWidth, TileCalcs.tileHeight);
                
                validArea.adjustArea(_tile.gridPosition);
                tilePrefabList.Add(_tile);       
            }
        }

        // Move ifs inside loop in future, as top, left, bottom, right is included in the map json
        foreach (var _tile in tilePrefabList) {
            if((int)_tile.gridPosition.x == validArea.maxX_left)
                _tile.activateLeftSide(true);
            if((int)_tile.gridPosition.y == validArea.maxY_bottom)
                _tile.activateRightSide(true);
        }

        // For different aspect-ratios
        TileCalcs.tileWidth = TileCalcs.tileWidth * Screen.width / 1920f;
        TileCalcs.tileHeight = TileCalcs.tileHeight * Screen.height / 1080f;

        Resources.UnloadUnusedAssets();
    }

    private GameObject getTilePrefab(int _isoX, int _isoY, float _spawnPosX, float _spawnPosY) {
        var _tile = mapJson.tiles.FirstOrDefault(_t => _t.pos.x == _isoX && _t.pos.y == _isoY);

        GameObject _prefab;

        if (loadedFromResources.ContainsKey(_tile.underlayTile))
            _prefab = loadedFromResources[_tile.underlayTile];
        else {
            Debug.Log($"Loaded prefab: {_tile.underlayTile}");
            _prefab = Resources.Load<GameObject>(_tile.underlayTile);
            loadedFromResources[_tile.underlayTile] = _prefab;
        }
        
        var _go = Instantiate(_prefab, new Vector3(_spawnPosX, _spawnPosY, 0), Quaternion.identity, mapParent);
        return _go;
    }
}
