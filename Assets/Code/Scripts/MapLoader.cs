using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI selected;
    [SerializeField] private TextMeshProUGUI mousePosText;
    [SerializeField] private TextMeshProUGUI insideRhombusText;
    [SerializeField] private TextMeshProUGUI rhombusInfo;
    [SerializeField] private Image rectangle;

    [SerializeField] private bool drawIsometricGrid = false;
    [SerializeField] private bool drawOrthographicGrid = false;
    
    [SerializeField] private Transform mapParent;
    
    private List<GameObject> tilePrefabList;

    private const float widthOffset = 1.28f, heightOffest = 0.64f;

    void Start() {
        tilePrefabList = new List<GameObject>();
        
        Debug.Log(lineSegmentsIntersect(new Vector2(0,0), new Vector2(2, 1), new Vector2(4, 0), new Vector2(0, 4)));
        Debug.Log(lineSegmentsIntersect(new Vector2(0,0), new Vector2(2, 1), new Vector2(1, 0), new Vector2(0, 2)));
        //Debug.Log(lineSegmentsIntersect(new Vector2(), new Vector2(), new Vector2(), new Vector2()));

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
    }

    private void Update() {
        var _mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        // multiply by 100 because of the 100 pixels per unit of Unity
        var _cellPos = new Vector2(Input.mousePosition.x / (100 * widthOffset), Input.mousePosition.y / (100 * heightOffest));
        var _cellOff = new Vector2(Input.mousePosition.x % widthOffset, Input.mousePosition.y % heightOffest);

        //var _realPos = getRealCell(_cellPos);
        
        // Calc in grid space
        var _selectedCell = new Vector2((_cellPos.y - tileCenter.y) + (_cellPos.x - tileCenter.x), (_cellPos.y - tileCenter.y) - (_cellPos.x - tileCenter.x));
        var _selectedCellGrid = toGrid((int)_selectedCell.x, (int)_selectedCell.y);

        Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        selected.text = $"X: {(int)_cellPos.x}, Y: {(int)_cellPos.y}";
        mousePosText.text = $"X: {_mousePos.x}, Y: {_mousePos.y}";
        //insideRhombusText.text = $"Is inside rhombus? {getRealCell(new Vector2((int)_cellPos.x, (int)_cellPos.y))}";

        rectangle.transform.localPosition = new Vector3(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f);
        getRealCell(new Vector2((int)_cellPos.x, (int)_cellPos.y));
        #if UNITY_EDITOR
        if(drawIsometricGrid) drawGridIsometric(Color.green);
        if(drawOrthographicGrid) drawGridOrthographic(Color.magenta);
        #endif
    }

    private void getRealCell(Vector2 _cellPos) {
        // _cellPos is the left-lower corner of the rectangle of the tile, we need to calc all four vertices of the rhombus (A, B, C, D), and then, the center (Q)
        // With that, we will calc if AB, BC, CD, DA intersects with QMosePos. If no intersection, the mouse is over the rhombus inside the rectangle, else
        //    - if AB, the mouse is over the rhombus on upper-left
        //    - if BC, the mouse is over the rhombus on upper-right
        //    - if CD, the mouse is over the rhombus on lower-right
        //    - if DA, the mouse is over the rhombus on lower-left

        _cellPos.x *= widthOffset * 100;
        _cellPos.y *= heightOffest * 100;
        
        var A = new Vector2(_cellPos.x, _cellPos.y + heightOffest / 2f * 100);
        var B = new Vector2(_cellPos.x + widthOffset / 2f * 100, _cellPos.y + heightOffest * 100);
        var C = new Vector2(_cellPos.x + widthOffset * 100, _cellPos.y + heightOffest / 2f * 100);
        var D = new Vector2(_cellPos.x + widthOffset / 2f * 100, _cellPos.y);
        
        var Q = new Vector2(_cellPos.x + widthOffset / 2f * 100, _cellPos.y + heightOffest / 2f * 100);
        var P = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        rhombusInfo.text = $"[A{A}, B{B}, C{C}, D{D}, Q{Q}, P{P}] ";
        
        Debug.DrawLine(new Vector3((Q.x - Screen.width / 2f) / 100f, (Q.y - Screen.height / 2f) / 100f), new Vector3(
            (P.x - Screen.width / 2f) / 100f, (P.y - Screen.height / 2f) / 100f), Color.red);
        
        // Rhombus
        Debug.DrawLine(new Vector3((A.x - Screen.width / 2f) / 100f, (A.y - Screen.height / 2f) / 100f), new Vector3(
            (B.x - Screen.width / 2f) / 100f, (B.y - Screen.height / 2f) / 100f), Color.green);
        
        Debug.DrawLine(new Vector3((B.x - Screen.width / 2f) / 100f, (B.y - Screen.height / 2f) / 100f), new Vector3(
            (C.x - Screen.width / 2f) / 100f, (C.y - Screen.height / 2f) / 100f), Color.green);
        
        Debug.DrawLine(new Vector3((C.x - Screen.width / 2f) / 100f, (C.y - Screen.height / 2f) / 100f), new Vector3(
            (D.x - Screen.width / 2f) / 100f, (D.y - Screen.height / 2f) / 100f), Color.green);
        
        Debug.DrawLine(new Vector3((D.x - Screen.width / 2f) / 100f, (D.y - Screen.height / 2f) / 100f), new Vector3(
            (A.x - Screen.width / 2f) / 100f, (A.y - Screen.height / 2f) / 100f), Color.green);

        
        // Rectangle
        Debug.DrawLine(new Vector3(_cellPos.x, _cellPos.y), new Vector3(_cellPos.x, _cellPos.y + heightOffest), Color.yellow);
        Debug.DrawLine(new Vector3(_cellPos.x, _cellPos.y), new Vector3(_cellPos.x, _cellPos.y + heightOffest), Color.yellow);
        Debug.DrawLine(new Vector3(_cellPos.x, _cellPos.y), new Vector3(_cellPos.x, _cellPos.y + heightOffest), Color.yellow);
        Debug.DrawLine(new Vector3(_cellPos.x, _cellPos.y), new Vector3(_cellPos.x, _cellPos.y + heightOffest), Color.yellow);
        
        if (lineSegmentsIntersect(A, B, Q, P)) {
            rhombusInfo.text += "intersects in AB";
            //return false;
        }
        
        if (lineSegmentsIntersect(B, C, Q, P)) {
            rhombusInfo.text += "intersects in BC";
            //return false;
        }
        
        if (lineSegmentsIntersect(C, D, Q, P)) {
            rhombusInfo.text += "intersects in CD";
            //return false;
        }
        
        if (lineSegmentsIntersect(D, A, Q, P)) {
            rhombusInfo.text += "intersects in DA";
            //return false;
        }

        //return true;
    }
    
    private void drawGridOrthographic(Color _color) {
        var _rows = 100;
        var _columns = 100;

        var _cam = Camera.main;
        var _height = 2f * _cam.orthographicSize;
        var _width = _height * _cam.aspect;
        
        for (var _r = 0; _r < _rows; _r++)
            Debug.DrawLine(new Vector3(-_width / 2f, heightOffest * _r - heightOffest * 15.5f), new Vector3(_width / 2f, heightOffest * _r - heightOffest * 15.5f), _color);

        for (var _c = 0; _c < _columns; _c++) 
            Debug.DrawLine(new Vector3(widthOffset * _c - widthOffset * 15.5f, -heightOffest * 15.5f), new Vector3(widthOffset * _c - widthOffset * 15.5f, heightOffest * 15.5f, 0), _color);
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
        var _tileY = -(_x + _y) * (heightOffest / 2f);

        _tileX += tileCenter.x * (widthOffset);
        _tileY -= tileCenter.y * (heightOffest);
        
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
