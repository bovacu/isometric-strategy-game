using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

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

    [FormerlySerializedAs("tilePrefab")] [SerializeField] private GameObject placeHolderPrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Transform mapParent;

    private Dictionary<string, GameObject> loadedFromResources = new Dictionary<string, GameObject>();

    private void Awake() {
        GameConfig.initGameData();
        Locize.initLocized();
        
        LoadMap();
    }

    private void LoadMap() {
        Debug.Log($"Aspect Ratio: {(float)Screen.height / (float)Screen.width}, w: {Screen.width}, h: {Screen.height}");
        Debug.Log($"Tile Width: {TileCalcs.tileWidth}, Tile height: {TileCalcs.tileHeight}");

        var _mapUrl = $"{Application.streamingAssetsPath}/Data/map.json";
        var _sr = new StreamReader(_mapUrl);
        var _json = _sr.ReadToEnd();
        _sr.Close();
        Map.MapInfo = JsonConvert.DeserializeObject<Map>(_json);

        if (Map.MapInfo == null) {
            Debug.LogError("Fatal error: mapInfo was null");
            return;
        }
        
        var _center = new Vector2(Map.MapInfo.info.width / 2f, Map.MapInfo.info.height / 2f);
        if (_center.x % 2 != 0)
            _center.x--;
        
        if (_center.y % 2 != 0)
            _center.y--;

        TileCalcs.tileCenter = _center;

        for (var _y = 0; _y < Map.MapInfo.info.height; _y++) {
            for (var _x = 0; _x < Map.MapInfo.info.width; _x++) {
                var _tilePos = TileCalcs.toGrid(_x, _y);
                var _spawnOrigin = _tilePos * 100;
                var _orthographicPos = new Vector2(_spawnOrigin.x / (100 * TileCalcs.tileWidth), _spawnOrigin.y / (100 * TileCalcs.tileHeight));
                var _isometricPos = TileCalcs.getRealCell(_orthographicPos, _spawnOrigin, Vector2.zero);
                
                var _cell = getTilePrefab((int)_isometricPos.x, (int)_isometricPos.y, _tilePos.x, _tilePos.y);
                _cell.mapCellJson.pos = _isometricPos;
                _cell.setDebugText(_isometricPos);
                _cell.size = new Vector2(TileCalcs.tileWidth, TileCalcs.tileHeight);
                _cell.setLayer(Map.MapInfo.info.width * Map.MapInfo.info.height - (_y * Map.MapInfo.info.width + _x));
                
                Map.MapInfo.validArea.adjustArea(_cell.mapCellJson.pos);
                Map.MapInfo.mapCellPrefabs.Add(_cell);
            }
        }

        // For different aspect-ratios
        TileCalcs.tileWidth = TileCalcs.tileWidth * Screen.width / 1920f;
        TileCalcs.tileHeight = TileCalcs.tileHeight * Screen.height / 1080f;

        Resources.UnloadUnusedAssets();
    }

    private Cell getTilePrefab(int _isoX, int _isoY, float _spawnPosX, float _spawnPosY) {
        var _mapCellJson = Map.MapInfo.jsonTiles.FirstOrDefault(_t => _t.pos.x == _isoX && _t.pos.y == _isoY);
        GameObject _prefab;

        if (loadedFromResources.ContainsKey(_mapCellJson.underlayTile))
            _prefab = loadedFromResources[_mapCellJson.underlayTile];
        else {
            Debug.Log($"Loaded prefab: {_mapCellJson.underlayTile}");
            _prefab = Resources.Load<GameObject>(_mapCellJson.underlayTile);

            if (_prefab == null) {
                Debug.LogError($"Tried to load prefab '{_mapCellJson.underlayTile}', but failed, instantiating placeholder");
                _prefab = placeHolderPrefab;
            }

            loadedFromResources[_mapCellJson.underlayTile] = _prefab;
        }
        
        var _go = Instantiate(_prefab, new Vector3(_spawnPosX, _spawnPosY, 0), Quaternion.identity, mapParent);
        var _cell = _go.GetComponent<Cell>();
        _cell.mapCellJson = _mapCellJson;
        
        instantiatePropOnCell(_go, new Vector2(_isoX, _isoY));
        
        return _cell;
    }

    private void instantiatePropOnCell(GameObject _parentCell, Vector2 _pos) {
        var _mapProplJson = Map.MapInfo.jsonProps.FirstOrDefault(_t => _t.pos.Equals(_pos));
        if (_mapProplJson != null) {
            var _binPref = Resources.Load(_mapProplJson.underlayTile) as GameObject;
            var _go = Instantiate(_binPref, mapParent);
            _go.GetComponent<RectTransform>().anchoredPosition = _parentCell.GetComponent<RectTransform>().anchoredPosition;
                    
            var _prop = _go.GetComponent<Prop>();
            _prop.setCell(_pos);
            _prop.setJsonInfo(GameConfig.propsInfo[_mapProplJson.id]);
            Debug.Log($"Loaded prop: {_prop.getJsonInfo().name}");
            _prop.setLayer(100);
            Map.MapInfo.props.Add(_prop);
        }
    }
}
