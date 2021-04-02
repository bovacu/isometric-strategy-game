using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapControls : MonoBehaviour {

    [Header("Camera")]
    [SerializeField] private Camera camera;

    [Header("Map")]
    [SerializeField] private GameObject map;
    [SerializeField] private MapLoader mapLoader;


    [Header("Debug")]
    [SerializeField] private TileDebugWindow tileDebugWindow;
    [SerializeField] private bool drawIsometricGrid = false;
    [SerializeField] private bool drawOrthographicGrid = false;
    [SerializeField] private bool activateCellDebugging = true;

    [Header("Control")]
    [SerializeField] private float extraMoveSpeed = 2f;
    
    private float minZoom = 1f, maxZoom = 5.4f;
    private float currentZoom = 0;

    private Vector2 initialClickPoint = Vector2.negativeInfinity;
    
    void Start() {
        currentZoom = maxZoom;
        tileDebugWindow.gameObject.SetActive(true);
    }
    
    void Update() {

        moveWithScrollButton(Time.deltaTime);

        var _localPosition = new Vector2(map.GetComponent<RectTransform>().localPosition.x, map.GetComponent<RectTransform>().localPosition.y);
        var _mousePosCentered = new Vector2((Input.mousePosition.x - Screen.width / 2f) * (currentZoom / maxZoom) - _localPosition.x,
            (Input.mousePosition.y - Screen.height / 2f) * (currentZoom / maxZoom) - _localPosition.y);

        zoom(camera, _mousePosCentered);
        
        var _cellPos = new Vector2(_mousePosCentered.x / (100 * TileCalcs.tileWidth), _mousePosCentered.y / (100 * TileCalcs.tileHeight));
        var _finalCell = TileCalcs.getRealCell(new Vector2(_cellPos.x, _cellPos.y), _mousePosCentered, _localPosition);

        leftClick(_finalCell);
        
        debug(_mousePosCentered, _finalCell);
    }

    private void moveWithScrollButton(float _delta) {
        var _current = map.transform.localPosition;

        if (Input.GetMouseButtonDown((int) MouseButton.RightMouse)) 
            initialClickPoint = new Vector2((Input.mousePosition.x - Screen.width / 2f) * (currentZoom / maxZoom), (Input.mousePosition.y - Screen.height / 2f) * (currentZoom / maxZoom));
        
        if (Input.GetMouseButtonUp((int) MouseButton.RightMouse)) 
            initialClickPoint = Vector2.negativeInfinity;
        
        if (!initialClickPoint.Equals(Vector2.negativeInfinity)) {
            var _P = new Vector2((Input.mousePosition.x - Screen.width / 2f) * (currentZoom / maxZoom), (Input.mousePosition.y - Screen.height / 2f) * (currentZoom / maxZoom));
            var _Q = initialClickPoint;
            var _PQ = _Q - _P;
            var _module = Mathf.Sqrt(_PQ.x * _PQ.x + _PQ.y * _PQ.y);
            _PQ /= _module; // Normalizing the vector
            
            if(!float.IsNaN(_PQ.x) && !float.IsNaN(_PQ.y))
                map.transform.localPosition = new Vector3(_current.x - _PQ.x * _module * _delta * extraMoveSpeed, _current.y - _PQ.y *  _module * _delta * extraMoveSpeed, _current.z);
        }
    }

    private void leftClick(Vector2 _finalCell) {
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) && mapLoader.validArea.mouseInsideMap(_finalCell)) {
            foreach (var _tile in mapLoader.tilePrefabList) {
                _tile.selectTile((int)_finalCell.x == (int)_tile.gridPosition.x && (int)_finalCell.y == (int)_tile.gridPosition.y);
            }
        }
    }
    
    private void zoom(Camera _cam, Vector2 _mousePos) {
        var ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        if (ScrollWheelChange != 0){
            var _zoom = _cam.orthographicSize;
            _zoom -= ScrollWheelChange;
            _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);
            _cam.orthographicSize = _zoom;
            currentZoom = _zoom;
            
            // map.transform.localPosition = new Vector3(_mousePos.x, _mousePos.y, map.transform.localPosition.z);
            tileDebugWindow.gameObject.transform.localScale = new Vector3(_zoom / maxZoom, _zoom / maxZoom, _zoom / maxZoom);
        }
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

    private void debug(Vector2 _mousePosCentered, Vector2 _finalCell) {
        if (mapLoader.validArea.mouseInsideMap(_finalCell)) {
            float _windowOffsetX = 96 * (currentZoom / maxZoom), _windowOffsetY = _windowOffsetX * (currentZoom / maxZoom);
            if(!tileDebugWindow.isActiveAndEnabled)
                tileDebugWindow.gameObject.SetActive(true);
            tileDebugWindow.gameObject.transform.localPosition = new Vector3(_mousePosCentered.x + _windowOffsetX, _mousePosCentered.y + _windowOffsetY);
            tileDebugWindow.tilePosIso.text = $"Iso: ({(int)_finalCell.x}, {(int)_finalCell.y})";
        } else {
            if(tileDebugWindow.isActiveAndEnabled)
                tileDebugWindow.gameObject.SetActive(false);
        }
        
        #if UNITY_EDITOR
            if(drawIsometricGrid) drawGridIsometric(Color.green);
            if(drawOrthographicGrid) drawGridOrthographic(Color.magenta);
        #endif

        TileCalcs.activateCellDebugging = activateCellDebugging;
    }

    public void centerMap() {
        map.transform.localPosition = Vector3.zero;
    }
}
