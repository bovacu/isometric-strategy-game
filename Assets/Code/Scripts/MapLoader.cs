using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public struct Vec2 {
    public int x, y;

    public Vec2(int _x, int _y) {
        x = _x;
        y = _y;
    }
}

public class MapLoader : MonoBehaviour {

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private Vector2 tileCenter;
    [SerializeField] private Image defaultTexture;
    [SerializeField] private TileDebugWindow tileDebugWindow;

    [SerializeField] private bool drawIsometricGrid = false;
    [SerializeField] private bool drawOrthographicGrid = false;

    [SerializeField] private bool activateCellDebugging = true;
    
    [SerializeField] private Transform mapParent;
    
    private List<GameObject> tilePrefabList;

    private const float widthOffset = 1.28f, heightOffest = 0.64f;
    
    

    void Start() {
        tilePrefabList = new List<GameObject>();

        for (var _y = 0; _y < mapSize.y; _y++) {
            for (var _x = 0; _x < mapSize.x; _x++) {
                var _tilePos = toGrid(_x, _y);

                var _go = Instantiate(tilePrefab, new Vector3(_tilePos.x, _tilePos.y), Quaternion.identity, mapParent);
                _go.GetComponent<SpriteRenderer>().sortingOrder = _y * (int)mapSize.x + _x;
                
                var _tile = _go.GetComponent<Tile>();
                _tile.gridPosition = new Vector2(_x, _y);
                _tile.originalSprite = defaultTexture;
                _tile.size = new Vector2(widthOffset, heightOffest);
                
                tilePrefabList.Add(_go);       
            }
        }
        
        if(activateCellDebugging)
            tileDebugWindow.gameObject.SetActive(true);
    }

    public void onReload() {
        foreach (Transform child in mapParent) {
            Destroy(child.gameObject);
        }
        Start();
    }
    
    private void Update() {
        // multiply by 100 because of the 100 pixels per unit of Unity
        var _mousePosCentered = new Vector2(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f);
        var _cellPos = new Vector2(_mousePosCentered.x / (100 * widthOffset), _mousePosCentered.y / (100 * heightOffest));
        tileDebugWindow.tileType.text = $"CellPos: ({_cellPos.x}, {_cellPos.y})";
        var _finalCell = getRealCell(new Vector2(_cellPos.x, _cellPos.y), _mousePosCentered);


        #if UNITY_EDITOR
            if(drawIsometricGrid) drawGridIsometric(Color.green);
            if(drawOrthographicGrid) drawGridOrthographic(Color.magenta);
        #endif
        
        tileDebugWindow.tilePos.text = $"CellposFixed: ({(int)_finalCell.x}, {(int)_finalCell.y})";
    }

    private Vector2 getRealCell(Vector2 _cellPos, Vector2 _mousePosCentered) {
        // _cellPos is the left-lower corner of the rectangle of the tile, we need to calc all four vertices of the rhombus (A, B, C, D), and then, the center (Q)
        // With that, we will calc if AB, BC, CD, DA intersects with QMosePos. If no intersection, the mouse is over the rhombus inside the rectangle, else
        //    - if AB, the mouse is over the rhombus on upper-left
        //    - if BC, the mouse is over the rhombus on upper-right
        //    - if CD, the mouse is over the rhombus on lower-right
        //    - if DA, the mouse is over the rhombus on lower-left

        var _collisionDebug = "";
        var _sideMultiplierX = _cellPos.x < 0 ? -1 : 1;
        var _sideMultiplierY = _cellPos.y < 0 ? -1 : 1;

        _cellPos.x = (int) _cellPos.x;
        _cellPos.y = (int) _cellPos.y;
        
        var _finalCell = new Vector2(_cellPos.x, _cellPos.y);
        
        if (_sideMultiplierX < 0)
            _finalCell.x--;
        
        if (_sideMultiplierY < 0)
            _finalCell.y--;
        
        _cellPos.x *= widthOffset * 100;
        _cellPos.y *= heightOffest * 100;

        var A = new Vector2(_cellPos.x, _cellPos.y + heightOffest / 2f * 100 * _sideMultiplierY);
        var B = new Vector2(_cellPos.x + widthOffset / 2f * 100 * _sideMultiplierX, _cellPos.y + heightOffest * 100 * _sideMultiplierY);
        var C = new Vector2(_cellPos.x + widthOffset * 100 * _sideMultiplierX, _cellPos.y + heightOffest / 2f * 100 * _sideMultiplierY);
        var D = new Vector2(_cellPos.x + widthOffset / 2f * 100 * _sideMultiplierX, _cellPos.y);
        
        var Q = new Vector2(_cellPos.x + widthOffset / 2f * 100 * _sideMultiplierX, _cellPos.y + heightOffest / 2f * 100 * _sideMultiplierY);
        var P = new Vector2(_mousePosCentered.x, _mousePosCentered.y);

        var _extraRestX = 0;
        var _extraRestY = 0;
        
        // Check intersections
        if (lineSegmentsIntersect(A, B, Q, P)) {
            // +1 on y
            _collisionDebug = "AB";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, -widthOffset / 2f * 100, heightOffest / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            if (_sideMultiplierX > 0) {
                _finalCell.y++;
                _extraRestX--;
            } else {
                _finalCell.x++;
                _extraRestY--;
            }

            if (_sideMultiplierY < 0) {
                _finalCell.y--;
            }

            Debug.Log("In AB");
        }
        
        if (lineSegmentsIntersect(B, C, Q, P)) {
            // +1 on x
            _collisionDebug = "BC";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, widthOffset / 2f * 100, heightOffest / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            if (_sideMultiplierX > 0) {
                _extraRestY++;
                _finalCell.y++;
            } else {
                _finalCell.y++;
                _extraRestX++;
            }

            Debug.Log($"In BC, x: {_finalCell.x}, y: {_finalCell.y}");
            
            if (_sideMultiplierY < 0) {
                _finalCell.y -= 3;
                _extraRestX += 2;
            }
        }
        
        if (lineSegmentsIntersect(C, D, Q, P)) {
            // -1 on x
            _collisionDebug = "CD";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, widthOffset / 2f * 100, -heightOffest / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            if (_sideMultiplierX > 0) {
                _finalCell.y--;
                _extraRestX++;
            } else {
                _finalCell.x--;
                _extraRestY++;
            }
            
            if (_sideMultiplierY < 0) {
                _finalCell.x++;
                _extraRestY += 2;
            }
            
            Debug.Log("In CD");
        }
        
        if (lineSegmentsIntersect(D, A, Q, P)) {
            // -1 on y
            _collisionDebug = "DA";
            correctValues(out A, out B, out C, out D, out Q, _cellPos, -widthOffset / 2f * 100, -heightOffest / 2f * 100, _sideMultiplierX, _sideMultiplierY);
            Debug.Log("In DA");
            
            if (_sideMultiplierX > 0) {
                _finalCell.x--;
                _extraRestY++;
            } else {
                _extraRestX--;
                _finalCell.y--;
            }
            
            if (_sideMultiplierY < 0) {
                _finalCell.y--;
                _extraRestX += 2;
            }
        }

        if (activateCellDebugging) {
            // Cell outline
            Debug.DrawLine(new Vector3(A.x / 100f, A.y / 100f), new Vector3(B.x / 100f, B.y / 100f), !_collisionDebug.Contains("AB") ? Color.green : Color.red);
            Debug.DrawLine(new Vector3(B.x / 100f, B.y / 100f), new Vector3(C.x / 100f, C.y / 100f), !_collisionDebug.Contains("BC") ? Color.green : Color.red);
            Debug.DrawLine(new Vector3(C.x / 100f, C.y / 100f), new Vector3(D.x / 100f, D.y / 100f), !_collisionDebug.Contains("CD") ? Color.green : Color.red);
            Debug.DrawLine(new Vector3(D.x / 100f, D.y / 100f), new Vector3(A.x / 100f, A.y / 100f), !_collisionDebug.Contains("DA") ? Color.green : Color.red);
        
            // Line from Q to mouse pos
            Debug.DrawLine(new Vector3(Q.x / 100f, Q.y / 100f), new Vector3(P.x / 100f, P.y / 100f), Color.red);
            
            int _windowOffsetX = 96, _windowOffsetY = _windowOffsetX;
            tileDebugWindow.gameObject.transform.localPosition = new Vector3(_mousePosCentered.x + _windowOffsetX, _mousePosCentered.y + _windowOffsetY);
        }

        return new Vector2(_finalCell.x + _finalCell.y + _extraRestX * _sideMultiplierX, _finalCell.y - _finalCell.x - _extraRestY * _sideMultiplierY);
    }

    private void correctValues(out Vector2 A, out Vector2 B, out Vector2 C, out Vector2 D, out Vector2 Q, Vector2 _cellInitPos, float _xCorrection, float _yCorrection, int _xSideM, int _ySideM) {
        A = new Vector2(_cellInitPos.x + _xCorrection * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + heightOffest / 2f * 100 * _ySideM);
        B = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + widthOffset / 2f * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + heightOffest * 100 * _ySideM);
        C = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + widthOffset * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + heightOffest / 2f * 100 * _ySideM);
        D = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + widthOffset / 2f * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM);
        Q = new Vector2(_cellInitPos.x + _xCorrection * _xSideM + widthOffset / 2f * 100 * _xSideM, _cellInitPos.y + _yCorrection * _ySideM + heightOffest / 2f * 100 * _ySideM);
    }
    
    private void drawGridOrthographic(Color _color) {
        var _rows = 100;
        var _columns = 100;

        var _cam = Camera.main;
        var _height = 2f * _cam.orthographicSize;
        var _width = _height * _cam.aspect;
        
        for (var _r = 0; _r < _rows; _r++)
            Debug.DrawLine(new Vector3(-_width / 2f, heightOffest * _r - heightOffest * 15f), new Vector3(_width / 2f, heightOffest * _r - heightOffest * 15f), _color);

        for (var _c = 0; _c < _columns; _c++) 
            Debug.DrawLine(new Vector3(widthOffset * _c - widthOffset * 15f, -heightOffest * 15f), new Vector3(widthOffset * _c - widthOffset * 15f, heightOffest * 15f, 0), _color);
    }
    
    private void drawGridIsometric(Color _color) {
        var _rows = 100;
        var _columns = 100;

        var _cam = Camera.main;
        var _height = 2f * _cam.orthographicSize;
        var _width = _height * _cam.aspect;
        
        for (var _r = 0; _r < _rows; _r++)
            Debug.DrawLine(new Vector3(-_width / 2f, heightOffest * _r - heightOffest * 15 - heightOffest * 10.5f), new Vector3(_width / 2f, heightOffest * _r - heightOffest * 45 - heightOffest * 10.5f), _color);

        for (var _c = 0; _c < _columns; _c++) 
            Debug.DrawLine(new Vector3(widthOffset * _c - widthOffset * 45 - widthOffset * 15.5f, -heightOffest * 15), new Vector3(widthOffset * _c - widthOffset * 15 - widthOffset * 15.5f, heightOffest * 15, 0), _color);
    }

    private Vector2 toGrid(int _x, int _y) {
        var _tileX = (_x - _y ) * (widthOffset / 2f);
        var _tileY = (_x + _y) * (heightOffest / 2f);

        _tileX += widthOffset / 2f;
        _tileY += heightOffest / 2f;
        
        _tileX += tileCenter.x * (widthOffset);
        _tileY += -tileCenter.y * (heightOffest);
        
        return new Vector2(_tileX, _tileY);
    }

    public static bool lineSegmentsIntersect(Vector2 lineOneA, Vector2 lineOneB, Vector2 lineTwoA, Vector2 lineTwoB) {
        return (((lineTwoB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x) > (lineTwoA.y - lineOneA.y) * (lineTwoB.x - lineOneA.x))
            != ((lineTwoB.y - lineOneB.y) * (lineTwoA.x - lineOneB.x) > (lineTwoA.y - lineOneB.y) * (lineTwoB.x - lineOneB.x))
            
            && 
            
            ((lineTwoA.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoA.x - lineOneA.x)) 
            != ((lineTwoB.y - lineOneA.y) * (lineOneB.x - lineOneA.x) > (lineOneB.y - lineOneA.y) * (lineTwoB.x - lineOneA.x)));
    }
}
