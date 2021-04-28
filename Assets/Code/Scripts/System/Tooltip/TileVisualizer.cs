using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class TileVisualizer : MonoBehaviour {

    [SerializeField] private GameObject panel;
    [SerializeField] private Transform parentForPrefabs;
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI status;
    [SerializeField] private TextMeshProUGUI health;
    
    private Dictionary<Vector2, Cell> roomTiles;
    private Vector2 lastShown = Vector2.negativeInfinity;

    private int closeAnim;
    private bool closing = false;
    
    public void setupForRoom() {
        if(roomTiles != null)
            foreach (var _prefab in roomTiles) 
                Destroy(_prefab.Value);

        roomTiles = new Dictionary<Vector2, Cell>();
        
        var _loadedPrefabs = new Dictionary<string, Cell>();
        
        Debug.Log("For tile visualizer, loaded: ");
        // Get all prefabs from the mapJson, and create them or reuse them if they are repeated over the map
        foreach (var _cell in Map.MapInfo.mapCellPrefabs) {
            if (_loadedPrefabs.ContainsKey(_cell.mapCellJson.underlayTile)) {
                roomTiles[_cell.mapCellJson.pos] = _loadedPrefabs[_cell.mapCellJson.underlayTile];
            } else {
                var _prefab = Instantiate(_cell, parentForPrefabs);
                _prefab.gameObject.SetActive(false);
                _prefab.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                _prefab.transform.localPosition = new Vector3(0, 0, 0f);
                _prefab.mapCellJson = _cell.mapCellJson;
                _prefab.upSide.GetComponent<SpriteRenderer>().sortingOrder = 2001;
                roomTiles[_cell.mapCellJson.pos] = _prefab;
                _loadedPrefabs[_cell.mapCellJson.underlayTile] = _prefab;
                Debug.Log($"    {_prefab.mapCellJson.underlayTile}");
            }
        }
        
        panel.transform.localScale = new Vector3(panel.transform.localScale.x, 0, panel.transform.localScale.z);
    }

    void setTexts(int _cellId) {
        var _cellVisualizerInfo = Locize.cellVisualizer.cells[GameConfig.cellsInfo[_cellId].id];
        var _cellVisualizerVisualizerConfig = Locize.cellVisualizer.visualizerConfig;

        var _header = Locize.substituteId(_cellVisualizerVisualizerConfig.header, GameConfig.cellsInfo[_cellId].name);
        header.text = string.Concat(_header, ": ", Locize.translateHashtags(_cellVisualizerInfo.mechanic));

        var _statusId = GameConfig.cellsInfo[_cellId].applyStatus.statusId;
        string _statusName;

        if (_statusId > 0)
            _statusName = GameConfig.statusInfo[_statusId].name;
        else
            _statusName = "---"; 
        
        var _status = Locize.substituteId(Locize.translateHashtags(_cellVisualizerVisualizerConfig.effect), _statusName);
        var _statusAndProb = _status;

        if (GameConfig.cellsInfo[_cellId].applyStatus.probability > 0)
            _statusAndProb = Locize.substituteId(_status, $"({GameConfig.cellsInfo[_cellId].applyStatus.probability * 100}%)");
        else
            _statusAndProb = Locize.substituteId(_status, "");
        
        status.text = _statusAndProb;
        
        var _cellDamage = GameConfig.cellsInfo[_cellId].damage;
        var _damage = Locize.substituteId(Locize.translateHashtags(_cellVisualizerVisualizerConfig.damage), 
            _cellDamage > 0 ? $"-{_cellDamage}" : _cellDamage.ToString());
        health.text = _damage;
    }
    
    public void update(Vector2 _currentPos) {
        if (Map.MapInfo.validArea.mouseInsideMap(_currentPos)) {
            
            if (!panel.activeInHierarchy || closing) {
                closing = false;
                LeanTween.cancel(closeAnim);
                LeanTween.scaleY(panel, 1, 0.15f).setEase(LeanTweenType.easeInSine);
            }
            
            panel.SetActive(true);
            if (!lastShown.Equals(Vector2.negativeInfinity)) {
                roomTiles[lastShown].gameObject.SetActive(false);
                roomTiles[_currentPos].gameObject.SetActive(true);
                lastShown = _currentPos;
            } else {
                roomTiles[_currentPos].gameObject.SetActive(true);
                lastShown = _currentPos;
            }
            
            setTexts(roomTiles[_currentPos].mapCellJson.id);
        } else {
            if (panel.transform.localScale.y != 0 && !closing) {
                closing = true;
                closeAnim = LeanTween.scaleY(panel, 0, 0.15f).setEase(LeanTweenType.easeOutSine).setOnComplete(() => {
                    panel.SetActive(false);
                    closing = false;
                }).id;
            }
        }
    }
}