using UnityEngine;
using UnityEngine.UIElements;

public class MapControls : MonoBehaviour {

    [Header("Camera")]
    [SerializeField] private Camera camera;

    [Header("Map")]
    [SerializeField] private GameObject map;
    [SerializeField] private MapLoader mapLoader;

    [Header("Selected")]
    [SerializeField] private GameObject selectedPrefab;

    [Header("Debug")]
    [SerializeField] private bool drawIsometricGrid = false;
    [SerializeField] private bool drawOrthographicGrid = false;
    [SerializeField] private bool activateCellDebugging = true;

    [Header("Control")]
    [SerializeField] private float extraMoveSpeed = 2f;
    
    private float minZoom = 1f, maxZoom = 5.4f;
    private float currentZoom = 5.4f;

    private Vector2 initialClickPoint = Vector2.negativeInfinity;
    private GameObject currentSelectedPrefab;
    
    private Vector2 mouseIconCorrection = new Vector2(5, 5);

    public static Vector2 currentSelectedTile;
    
    void Start() {
        currentSelectedTile = Vector2.negativeInfinity;
        currentZoom = maxZoom;
    }
    
    void Update() {

        if(!Console.consoleActive)
            keyboard();
        
        moveWithScrollButton(Time.deltaTime);

        var _mapOffset = new Vector2(map.GetComponent<RectTransform>().localPosition.x, map.GetComponent<RectTransform>().localPosition.y);
        var _mousePosCentered = getMousePosFixed(_mapOffset);

        zoom(camera, _mousePosCentered);
        
        var _cellPos = new Vector2(_mousePosCentered.x / (100 * TileCalcs.tileWidth), _mousePosCentered.y / (100 * TileCalcs.tileHeight));
        var _finalCell = TileCalcs.getRealCell(new Vector2(_cellPos.x, _cellPos.y), _mousePosCentered, _mapOffset);

        leftClick(_finalCell);
        
        debug(new Vector2(_mousePosCentered.x + _mapOffset.x, _mousePosCentered.y + _mapOffset.y), _finalCell);
    }

    private void keyboard() {
        centerMap();
        // debugShake();
    }

    private void debugShake() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            float height = Mathf.PerlinNoise(9.5f, 0f)*2.5f;
            float shakeAmt = height*0.1f; // the degrees to shake the camera
            float shakePeriodTime = 0.22f; // The period of each shake
            float dropOffTime = 0.6f; // How long it takes the shaking to settle down to nothing
            LTDescr shakeTween = LeanTween.rotateAroundLocal( gameObject, Vector3.up, shakeAmt, shakePeriodTime)
                .setEase( LeanTweenType.easeShake ) // this is a special ease that is good for shaking
                .setLoopClamp()
                .setRepeat(-1);

            // Slow the camera shake down to zero
            LeanTween.value(gameObject, (float val)=>{
                shakeTween.setTo(Vector3.right*val);
            }, shakeAmt, 0f, dropOffTime).setEase(LeanTweenType.easeOutQuad);
        }
    }

    private Vector2 getMousePosFixed(Vector2 _mapOffset) {
        // - Screen.* for centering mouse to middle of screen
        // * (currentZoom / maxZoom) to correct the distance that the mouse travels when zooming in/out
        // Last rest for different aspect-ratios
        return new Vector2((Input.mousePosition.x - Screen.width / 2f - mouseIconCorrection.x) * (currentZoom / maxZoom) - (_mapOffset.x * Screen.width / 1920f),
            (Input.mousePosition.y - Screen.height / 2f - mouseIconCorrection.y) * (currentZoom / maxZoom) - (_mapOffset.y * Screen.height / 1080f));
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
        selectTile(_finalCell);
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse)) {
            doAction(_finalCell);
        }
    }

    private void doAction(Vector2 _finalCell) {
        RoomManager.doAction(_finalCell, () => selectTile(Vector2.negativeInfinity));
    }
    
    private void zoom(Camera _cam, Vector2 _mousePos) {
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

    private void debug(Vector2 _mousePosCentered, Vector2 _finalCell) {
        #if UNITY_EDITOR
            if(drawIsometricGrid) drawGridIsometric(Color.green);
            if(drawOrthographicGrid) drawGridOrthographic(Color.magenta);
        #endif

        TileCalcs.activateCellDebugging = activateCellDebugging;
    }

    private void centerMap() {
        if(Input.GetKeyDown(KeyCode.C))
            map.transform.localPosition = Vector3.zero;
    }

    public void selectTile(Vector2 _finalCell) {
        if (_finalCell != currentSelectedTile) {
            if (Map.MapInfo.validArea.mouseInsideMap(_finalCell)) {
                if(!selectedPrefab.activeInHierarchy)
                    selectedPrefab.SetActive(true);
                // if(currentSelectedPrefab != null)
                //     Destroy(currentSelectedPrefab);
                
                foreach (var _tile in Map.MapInfo.mapCellPrefabs) {
                    if ((int) _finalCell.x == (int) _tile.mapCellJson.pos.x && (int) _finalCell.y == (int) _tile.mapCellJson.pos.y) {
                        // currentSelectedPrefab = Instantiate(selectedPrefab, _tile.transform);
                        selectedPrefab.transform.localPosition = _tile.transform.localPosition;
                        currentSelectedTile = _finalCell;
                        return;
                    }
                }
            } else {
                // if(currentSelectedPrefab != null)
                //     Destroy(currentSelectedPrefab);
                if(selectedPrefab.activeInHierarchy)
                    selectedPrefab.SetActive(false);
                currentSelectedTile = Vector2.negativeInfinity;
            }
        }
    }
}
