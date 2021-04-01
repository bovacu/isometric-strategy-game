using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapControls : MonoBehaviour {

    [SerializeField] private Camera camera;
    [SerializeField] private GameObject map;
    
    [SerializeField] private TileDebugWindow tileDebugWindow;
    [SerializeField] private bool drawIsometricGrid = false;
    [SerializeField] private bool drawOrthographicGrid = false;
    [SerializeField] private bool activateCellDebugging = true;

    [SerializeField] private MapLoader mapLoader;
    
    private float minZoom = 1f, maxZoom = 5.4f;
    private float currentZoom = 0;

    private float moveSpeed = 50f;
    
    // Start is called before the first frame update
    void Start() {
        currentZoom = maxZoom;
        tileDebugWindow.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        // deselectAll();
        
        moveWithScrollButton(Time.deltaTime);
        zoom(camera);
        
        var _localPosition = new Vector2(map.GetComponent<RectTransform>().localPosition.x, map.GetComponent<RectTransform>().localPosition.y);
        var _mousePosCentered = new Vector2((Input.mousePosition.x - Screen.width / 2f) * (currentZoom / maxZoom) - _localPosition.x,
            (Input.mousePosition.y - Screen.height / 2f) * (currentZoom / maxZoom) - _localPosition.y);

        var _cellPos = new Vector2(_mousePosCentered.x / (100 * TileCalcs.tileWidth), _mousePosCentered.y / (100 * TileCalcs.tileHeight));
        var _finalCell = TileCalcs.getRealCell(new Vector2(_cellPos.x, _cellPos.y), _mousePosCentered, _localPosition);

        leftClick(_finalCell);
        
        debug(_mousePosCentered, _finalCell, _cellPos);
    }

    private void moveWithScrollButton(float _delta) {
        var _current = map.transform.localPosition;
        if (Input.GetKey(KeyCode.A)) {
            map.transform.localPosition = new Vector3(_current.x + moveSpeed * _delta, _current.y, _current.z);
        } else if (Input.GetKey(KeyCode.D)) {
            map.transform.localPosition = new Vector3(_current.x - moveSpeed * _delta, _current.y, _current.z);
        }
        
        if (Input.GetKey(KeyCode.W)) {
            map.transform.localPosition = new Vector3(_current.x, _current.y - moveSpeed * _delta, _current.z);
        } else if (Input.GetKey(KeyCode.S)) {
            map.transform.localPosition = new Vector3(_current.x, _current.y + moveSpeed * _delta, _current.z);
        }
    }

    private void leftClick(Vector2 _finalCell) {
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse)) {
            foreach (var _tile in mapLoader.tilePrefabList) {
                _tile.selectTile((int)_finalCell.x == (int)_tile.gridPosition.x && (int)_finalCell.y == (int)_tile.gridPosition.y);
            }
        }
    }
    
    private void zoom(Camera _cam) {
        var ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        if (ScrollWheelChange != 0){
            var _zoom = _cam.orthographicSize;
            _zoom -= ScrollWheelChange;
            _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);
            _cam.orthographicSize = _zoom;
            currentZoom = _zoom;
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

    private void debug(Vector2 _mousePosCentered, Vector2 _finalCell, Vector2 _cellPos) {
        int _windowOffsetX = 96, _windowOffsetY = _windowOffsetX;
        tileDebugWindow.gameObject.transform.localPosition = new Vector3(_mousePosCentered.x + _windowOffsetX, _mousePosCentered.y + _windowOffsetY);
        tileDebugWindow.tilePosIso.text = $"Iso: ({(int)_finalCell.x}, {(int)_finalCell.y})";
        tileDebugWindow.tilePosOrtho.text = $"MousePos: ({_mousePosCentered.x}, {_mousePosCentered.y})";
        
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
