using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DoActionManager {

    struct ActionData {
        public RoomManager roomManager;
        public Vector2 finalCell;
        public int cost;
    }
    
    private delegate bool gameAction(ActionData _data, Action _onEnd = null);
    private Dictionary<NextAction, gameAction> actions;

    public void initDoActionManager() {
        actions = new Dictionary<NextAction, gameAction> {
            {NextAction.MOVE, basicMove},
            {NextAction.MELEE, basicMelee},
            {NextAction.RANGE, basicRange},
            {NextAction.DEFENSE, basicDefense}
        };
    }

    public bool doAction(RoomManager _roomManager, Vector2 _finalCell, NextAction _action, Action _onEnd = null) {
        if (!canActionBeDone(_roomManager, _finalCell, (int)_action)) {
            if(_roomManager.UserTarget.getEnergy() - GameConfig.basicMovements[(int)_action].cost < 0)
                _roomManager.clearTurn();
            
            return false;
        }

        var _data = new ActionData {
            roomManager = _roomManager,
            finalCell = _finalCell,
            cost = Console.infiniteEnergy ? 0 : GameConfig.basicMovements[(int)_action].cost
        };

        if (actions[_action](_data, _onEnd)) {
            _data.roomManager.UserTarget.setEnergy(_data.roomManager.UserTarget.getEnergy() - _data.cost);
            _roomManager.clearTurn(true);
            return true;
        }

        _roomManager.clearTurn();
        return false;
    }

    private bool canActionBeDone(RoomManager _roomManager, Vector2 _finalCell, int _action) {
        var _energyCost = GameConfig.basicMovements[_action].cost;
        var _cellOk = _roomManager.availableCells.Contains(_finalCell);
        if(!_cellOk && Map.MapInfo.validArea.mouseInsideMap(_finalCell))
            Debug.Log("Action cannot be done! Selected tile is not a valid one!");
        
        var _energyOk = _roomManager.UserTarget.getEnergy() - _energyCost >= 0;
        if(!_energyOk && Map.MapInfo.validArea.mouseInsideMap(_finalCell) && _roomManager.UserTarget.Equals(_roomManager.getPlayerData())) {
            _roomManager.getPlayerData().shakeIfEmptyEnergy();
            Debug.Log("Action cannot be done! You have not enough energy!");
        }
        
        Debug.Log($"CellOk: {_cellOk}, enetgyOk: {_energyOk}");
        
        return _cellOk && _energyOk;
    }

    private bool basicMove(ActionData _data, Action _onEnd = null) {
        _data.roomManager.UserTarget.moveAnim(_data.finalCell);
        _data.roomManager.UserTarget.setCell(_data.finalCell);

        Map.MapInfo.mapCellPrefabs.First(_c => _c.mapCellJson.pos.Equals(_data.finalCell)).interact(_data.roomManager.UserTarget);
        
        _data.roomManager.SetNextAction((int) NextAction.IDLE);
        
        _onEnd?.Invoke();
        
        return true;
    }
    
    private bool basicMelee(ActionData _data, Action _onEnd = null) {
        try {
            _data.roomManager.UserTarget.meleeAnim(_data.finalCell);
            var _target = _data.roomManager.RoomTargets.FirstOrDefault(_t => _t.getCell().Equals(_data.finalCell));
            _target.setHealth(_target.getHealth() - _data.roomManager.UserTarget.getMeleeAttack());
            _data.roomManager.SetNextAction((int)NextAction.IDLE);
            
            _onEnd?.Invoke();
            
            return true;
        } catch (Exception _e) {
            Debug.Log($"Error happened on basicMelee: {_e}");
            return false;
        }
    }
    
    private bool basicRange(ActionData _data, Action _onEnd = null) {
        try {
            _data.roomManager.UserTarget.rangeAnim(_data.finalCell);
            _data.roomManager.SetNextAction((int) NextAction.IDLE);
            
            _onEnd?.Invoke();
            
            return true;
        } catch (Exception _e) {
            Debug.Log($"Error happened on basicRange: {_e}");
            return false;
        }
    }
    
    private bool basicDefense(ActionData _data, Action _onEnd = null) {
        try {
            _data.roomManager.UserTarget.defenseAnim(_data.finalCell);
            _data.roomManager.SetNextAction((int)NextAction.IDLE);
            
            _onEnd?.Invoke();
            
            return true;
        } catch (Exception _e) {
            Debug.Log($"Error happened on basicDefense: {_e}");
            return false;
        }
    }
}