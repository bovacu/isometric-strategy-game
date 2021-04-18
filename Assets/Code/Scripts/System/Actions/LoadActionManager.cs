using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadActionManager {
    private delegate bool gameAction(RoomManager _roomManager, int _range, int _action);
    private Dictionary<RangeType, gameAction> actions;

    public void initLoadActionManager() {
        actions = new Dictionary<RangeType, gameAction> {
            {RangeType.CROSS, crossRange},
            {RangeType.INDIVIDUAL, individualRange},
            {RangeType.ALL_DIRECTION, allDirectionsRange}
        };
    }
    
    private Color debugColorSelectionForTiles(int _nextAction) {
        Color _color;
        switch (_nextAction) {
            case (int)NextAction.MOVE: _color = new Color(103 / 255f, 245 / 255f, 13 / 255f, 1);
                break;
            case (int)NextAction.MELEE: _color = new Color(1, 66 / 255f, 66 / 255f, 1);
                break;
            case (int)NextAction.RANGE: _color = new Color(193 / 255f, 54 / 255f, 1, 1);
                break;
            case (int)NextAction.DEFENSE: _color = new Color(1, 192 / 255f, 40 / 255f, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_nextAction), _nextAction, null);
        }

        return _color;
    }
    
    public bool loadAction(RoomManager _roomManager, int _range, RangeType _rangeType, int _action) {
        try {
            _roomManager.clearTurn();
            return actions[_rangeType](_roomManager, _range, _action);
        }
        catch (Exception e) {
            Debug.Log($"The action {_rangeType}({(int)_rangeType}) wasn't in the dictionary for the action {(NextAction)_action}, {e}");
        }

        return false;
    }

    private bool allDirectionsRange(RoomManager _roomManager, int _range, int _action) {
        var _color = debugColorSelectionForTiles(_action);
        var _targetCell = _roomManager.getPlayerData().currentCell;

        for (var _i = -_range; _i < _range; _i++) {
            for (var _j = -_range; _j < _range; _j++) {
                if(_i == 0 && _j == 0) continue;
                if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x + _j, _targetCell.y + _i))) {
                    Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int)_targetCell.x + _j &&
                                                     (int) _targetCell.y + _i == (int) _c.mapCellJson.pos.y).upSide.GetComponent<SpriteRenderer>().color = _color;
                    _roomManager.availableCells.Add(new Vector2(_targetCell.x + _j, _targetCell.y + _i));
                }
            }
        }

        return true;
    }

    private bool crossRange(RoomManager _roomManager, int _range, int _action) {
        var _color = debugColorSelectionForTiles(_action);
        var _targetCell = _roomManager.getPlayerData().currentCell;

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x + _i, _targetCell.y))) {
                Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x + _i &&
                                                 (int) _targetCell.y == (int) _c.mapCellJson.pos.y).upSide.GetComponent<SpriteRenderer>().color = _color;
                _roomManager.availableCells.Add(new Vector2(_targetCell.x + _i, _targetCell.y));
            }
        }

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x - _i, _targetCell.y))) {
                Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x - _i &&
                                                 (int) _targetCell.y == (int) _c.mapCellJson.pos.y).upSide.GetComponent<SpriteRenderer>().color = _color;
                _roomManager.availableCells.Add(new Vector2(_targetCell.x - _i, _targetCell.y));
            }
        }

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x, _targetCell.y + _i))) {
                Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x &&
                                                 (int) _targetCell.y + _i == (int) _c.mapCellJson.pos.y).upSide.GetComponent<SpriteRenderer>().color = _color;
                _roomManager.availableCells.Add(new Vector2(_targetCell.x, _targetCell.y + _i));
            }
        }

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x, _targetCell.y - _i))) {
                Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x &&
                                                 (int) _targetCell.y - _i == (int) _c.mapCellJson.pos.y).upSide.GetComponent<SpriteRenderer>().color = _color;
                _roomManager.availableCells.Add(new Vector2(_targetCell.x, _targetCell.y - _i));
            }
        }
        return true;
    }
    
    private bool individualRange(RoomManager _roomManager, int _range, int _action) {
        var _color = debugColorSelectionForTiles(_action);
        var _targetCell = _roomManager.getPlayerData().currentCell;
        _roomManager.availableCells.Add(_targetCell);
        Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x &&
                                         (int) _targetCell.y == (int) _c.mapCellJson.pos.y).upSide.GetComponent<SpriteRenderer>().color = _color;
        return true;
    }
    
    
}