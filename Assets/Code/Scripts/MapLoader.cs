using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour {

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Vector2 tileCenter;
    [SerializeField] private Image defaultTexture;

    [SerializeField] private Transform mapParent;
    
    private List<GameObject> tilePrefabList;

    void Start() {
        TileCalcs.tileCenter = tileCenter;
        
        tilePrefabList = new List<GameObject>();

        for (var _y = 0; _y < mapSize.y; _y++) {
            for (var _x = 0; _x < mapSize.x; _x++) {
                var _tilePos = TileCalcs.toGrid(_x, _y);

                var _go = Instantiate(tilePrefab, new Vector3(_tilePos.x, _tilePos.y), Quaternion.identity, mapParent);
                _go.GetComponent<SpriteRenderer>().sortingOrder = _y * (int)mapSize.x + _x;

                var _tile = _go.GetComponent<Tile>();
                var _rect = _go.GetComponent<RectTransform>().localPosition;
                _tile.gridPosition = TileCalcs.getRealCell(new Vector2(_rect.x / (100 * TileCalcs.tileWidth), _rect.y / 100 * TileCalcs.tileHeight), _rect);
                _tile.originalSprite = defaultTexture;
                _tile.size = new Vector2(TileCalcs.tileWidth, TileCalcs.tileHeight);
                
                tilePrefabList.Add(_go);       
            }
        }
    }

    public void onReload() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
        Start();
    }

}
