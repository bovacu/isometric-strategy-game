using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControls : MonoBehaviour {
    
    [SerializeField] private TileDebugWindow tileDebugWindow;
    [SerializeField] private bool drawIsometricGrid = false;
    [SerializeField] private bool drawOrthographicGrid = false;
    [SerializeField] private bool activateCellDebugging = true;

    private float minZoom = 1f, maxZoom = 5.4f;
    private float currentZoom = 0;
    
    // Start is called before the first frame update
    void Start() {
        currentZoom = maxZoom;
    }

    // Update is called once per frame
    void Update() {
        var _cam = Camera.main;
        // multiply by 100 because of the 100 pixels per unit of Unity
        var _mousePosCentered = new Vector2((Input.mousePosition.x - Screen.width / 2f) * (currentZoom / maxZoom),
            (Input.mousePosition.y - Screen.height / 2f) * (currentZoom / maxZoom));
        
        var _cellPos = new Vector2(_mousePosCentered.x / (100 * TileCalcs.tileWidth), _mousePosCentered.y / (100 * TileCalcs.tileHeight));
        var _finalCell = TileCalcs.getRealCell(new Vector2(_cellPos.x, _cellPos.y), _mousePosCentered);

        #if UNITY_EDITOR
                if(drawIsometricGrid) drawGridIsometric(Color.green);
                if(drawOrthographicGrid) drawGridOrthographic(Color.magenta);
        #endif

        var ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        if (ScrollWheelChange != 0){
            var _zoom = _cam.orthographicSize;
            _zoom -= ScrollWheelChange;
            _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);
            _cam.orthographicSize = _zoom;
            currentZoom = _zoom;
        }

        if (activateCellDebugging) {
            int _windowOffsetX = 96, _windowOffsetY = _windowOffsetX;
            tileDebugWindow.gameObject.SetActive(true);
            tileDebugWindow.gameObject.transform.localPosition = new Vector3(_mousePosCentered.x + _windowOffsetX, _mousePosCentered.y + _windowOffsetY);
            tileDebugWindow.tilePosIso.text = $"Iso: ({(int)_finalCell.x}, {(int)_finalCell.y})";
            tileDebugWindow.tilePosOrtho.text = $"Ortho: ({_cellPos.x}, {_cellPos.y})";
        }

        TileCalcs.activateCellDebugging = activateCellDebugging;
    }
    
    private void drawGridOrthographic(Color _color) {
        var _rows = 100;
        var _columns = 100;

        var _cam = Camera.main;
        var _height = 2f * _cam.orthographicSize;
        var _width = _height * _cam.aspect;
        
        for (var _r = 0; _r < _rows; _r++)
            Debug.DrawLine(new Vector3(-_width / 2f, TileCalcs.tileHeight * _r - TileCalcs.tileHeight * 15f), new Vector3(_width / 2f, TileCalcs.tileHeight * _r - TileCalcs.tileHeight * 15f), _color);

        for (var _c = 0; _c < _columns; _c++) 
            Debug.DrawLine(new Vector3(TileCalcs.tileWidth * _c - TileCalcs.tileWidth * 15f, -TileCalcs.tileHeight * 15f), new Vector3(TileCalcs.tileWidth * _c - TileCalcs.tileWidth * 15f, TileCalcs.tileHeight * 15f, 0), _color);
    }
    
    private void drawGridIsometric(Color _color) {
        var _rows = 100;
        var _columns = 100;

        var _cam = Camera.main;
        var _height = 2f * _cam.orthographicSize;
        var _width = _height * _cam.aspect;
        
        for (var _r = 0; _r < _rows; _r++)
            Debug.DrawLine(new Vector3(-_width / 2f, TileCalcs.tileHeight * _r - TileCalcs.tileHeight * 15 - TileCalcs.tileHeight * 10.5f), new Vector3(_width / 2f, TileCalcs.tileHeight * _r - TileCalcs.tileHeight * 45 - TileCalcs.tileHeight * 10.5f), _color);

        for (var _c = 0; _c < _columns; _c++) 
            Debug.DrawLine(new Vector3(TileCalcs.tileWidth * _c - TileCalcs.tileWidth * 45 - TileCalcs.tileWidth * 15.5f, -TileCalcs.tileHeight * 15), new Vector3(TileCalcs.tileWidth * _c - TileCalcs.tileWidth * 15 - TileCalcs.tileWidth * 15.5f, TileCalcs.tileHeight * 15, 0), _color);
    }
}
