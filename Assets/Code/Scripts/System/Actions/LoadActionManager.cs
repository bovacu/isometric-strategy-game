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
    
    private Color debugColorSelectionForTiles(int _nextAction, RoomManager _roomManager) {
        Color _color;
        if (_roomManager.UserTarget.getEnergy() - GameConfig.basicMovements[_nextAction].cost < 0) {
            _color = Color.red;
        } else
            _color = new Color(103 / 255f, 245 / 255f, 13 / 255f, 0.85f);

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

    void checkIfMovePossible(RoomManager _roomManager, Vector2 _finalPos, int _action, Cell _cell) {
        if(_action == (int)NextAction.MOVE)
            if (Map.MapInfo.props.FirstOrDefault(_p => _p.getCell().Equals(_finalPos)) != null ||
                _roomManager.RoomTargets.FirstOrDefault(_ai => _ai.getCell().Equals(_finalPos)) != null) {
                _cell.upSide.GetComponent<SpriteRenderer>().color = Color.white;
                _roomManager.availableCells.Remove(_finalPos);
            }
    }
    
    private bool allDirectionsRange(RoomManager _roomManager, int _range, int _action) {
        var _color = debugColorSelectionForTiles(_action, _roomManager);
        var _targetCell = _roomManager.UserTarget.getCell();

        for (var _i = -_range; _i <= _range; _i++) {
            for (var _j = -_range; _j <= _range; _j++) {
                if(_i == 0 && _j == 0) continue;
                if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x + _j, _targetCell.y + _i))) {
                    var _cell = Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int)_targetCell.x + _j &&
                                                     (int) _targetCell.y + _i == (int) _c.mapCellJson.pos.y);
                    _cell.upSide.GetComponent<SpriteRenderer>().color = _color;
                    
                    var _pos = new Vector2(_targetCell.x + _j, _targetCell.y + _i);
                    _roomManager.availableCells.Add(_pos);
                    checkIfMovePossible(_roomManager, _pos, _action, _cell);
                }
            }
        }

        return true;
    }

    private bool crossRange(RoomManager _roomManager, int _range, int _action) {
        var _color = debugColorSelectionForTiles(_action, _roomManager);
        var _targetCell = _roomManager.UserTarget.getCell();

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x + _i, _targetCell.y))) {
                var _cell = Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x + _i &&
                                                       (int) _targetCell.y == (int) _c.mapCellJson.pos.y);
                _cell.upSide.GetComponent<SpriteRenderer>().color = _color;
                var _pos = new Vector2(_targetCell.x + _i, _targetCell.y);
                _roomManager.availableCells.Add(_pos);
                checkIfMovePossible(_roomManager, _pos, _action, _cell);
            }
        }

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x - _i, _targetCell.y))) {
                var _cell = Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x - _i &&
                                                       (int) _targetCell.y == (int) _c.mapCellJson.pos.y);
                _cell.upSide.GetComponent<SpriteRenderer>().color = _color;
                var _pos = new Vector2(_targetCell.x - _i, _targetCell.y);
                _roomManager.availableCells.Add(_pos);
                checkIfMovePossible(_roomManager, _pos, _action, _cell);
            }
        }

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x, _targetCell.y + _i))) {
                var _cell = Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x &&
                                                       (int) _targetCell.y + _i == (int) _c.mapCellJson.pos.y);
                _cell.upSide.GetComponent<SpriteRenderer>().color = _color;
                var _pos = new Vector2(_targetCell.x, _targetCell.y + _i);
                _roomManager.availableCells.Add(_pos);
                checkIfMovePossible(_roomManager, _pos, _action, _cell);
            }
        }

        for (var _i = 1; _i <= _range; _i++) {
            if (Map.MapInfo.validArea.mouseInsideMap(new Vector2(_targetCell.x, _targetCell.y - _i))) {
                var _cell = Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x &&
                                                       (int) _targetCell.y - _i == (int) _c.mapCellJson.pos.y);
                _cell.upSide.GetComponent<SpriteRenderer>().color = _color;
                var _pos = new Vector2(_targetCell.x, _targetCell.y - _i);
                _roomManager.availableCells.Add(_pos);
                checkIfMovePossible(_roomManager, _pos, _action, _cell);
            }
        }
        return true;
    }
    
    private bool individualRange(RoomManager _roomManager, int _range, int _action) {
        var _color = debugColorSelectionForTiles(_action, _roomManager);
        var _targetCell = _roomManager.UserTarget.getCell();
        _roomManager.availableCells.Add(_targetCell);
        Map.MapInfo.mapCellPrefabs.First(_c => (int) _c.mapCellJson.pos.x == (int) _targetCell.x &&
                                         (int) _targetCell.y == (int) _c.mapCellJson.pos.y).upSide.GetComponent<SpriteRenderer>().color = _color;
        return true;
    }
    
    
}