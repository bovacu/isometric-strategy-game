using System;
using System.Collections.Generic;
using UnityEngine;

public class DoActionManager {

    struct ActionData {
        public RoomManager roomManager;
        public Vector2 finalCell;
        public int cost;
    }
    
    private delegate bool gameAction(ActionData _data);
    private Dictionary<NextAction, gameAction> actions;

    public void initDoActionManager() {
        actions = new Dictionary<NextAction, gameAction> {
            {NextAction.MOVE, basicMove},
            {NextAction.MELEE, basicMelee},
            {NextAction.RANGE, basicRange},
            {NextAction.DEFENSE, basicDefense}
        };
    }

    public bool doAction(RoomManager _roomManager, Vector2 _finalCell, NextAction _action) {
        if (!canActionBeDone(_roomManager, _finalCell, (int)_action)) 
            return false;

        var _data = new ActionData {
            roomManager = _roomManager,
            finalCell = _finalCell,
            cost = Console.infiniteEnergy ? 0 : GameConfig.basicMovements[(int)_action].cost
        };
        
        _roomManager.unloadAvailablePositions();

        if (actions[_action](_data)) {
            _roomManager.getPlayerData().updateEnergy(_roomManager.getPlayerData().currentEnergy);
            return true;
        }

        return false;
    }

    private bool canActionBeDone(RoomManager _roomManager, Vector2 _finalCell, int _action) {
        var _energyCost = GameConfig.basicMovements[_action].cost;
        var _cellOk = _roomManager.availableCells.Contains(_finalCell);
        if(!_cellOk)
            Debug.Log("Action cannot be done! Selected tile is not a valid one!");
        
        var _energyOk = _roomManager.getPlayerData().currentEnergy - _energyCost >= 0;
        if(!_energyOk)
            Debug.Log("Action cannot be done! You have not enough energy!");
        
        return _cellOk && _energyOk;
    }

    private bool basicMove(ActionData _data) {
        _data.roomManager.getPlayerData().updatePosToCellPos(_data.finalCell);
        _data.roomManager.getPlayerData().currentCell = _data.finalCell;
        _data.roomManager.SetNextAction((int) NextAction.IDLE);
        _data.roomManager.getPlayerData().currentEnergy -= _data.cost;
        return true;
    }
    
    private bool basicMelee(ActionData _data) {
        try {
            _data.roomManager.getPlayerData().updatePosToCellPosThenGoBack(_data.finalCell);
            _data.roomManager.SetNextAction((int)NextAction.IDLE);
            _data.roomManager.getPlayerData().currentEnergy -= _data.cost;
            return true;
        } catch (Exception _e) {
            Debug.Log($"Error happened on basicMelee: {_e}");
            return false;
        }
    }
    
    private bool basicRange(ActionData _data) {
        try {
            _data.roomManager.SetNextAction((int) NextAction.IDLE);
            _data.roomManager.getPlayerData().currentEnergy -= _data.cost;
            return true;
        } catch (Exception _e) {
            Debug.Log($"Error happened on basicRange: {_e}");
            return false;
        }
    }
    
    private bool basicDefense(ActionData _data) {
        try {
            _data.roomManager.getPlayerData().testDefenseTween();
            _data.roomManager.getPlayerData().currentCell = _data.finalCell;
            _data.roomManager.SetNextAction((int)NextAction.IDLE);
            _data.roomManager.getPlayerData().currentEnergy -= _data.cost;
            return true;
        } catch (Exception _e) {
            Debug.Log($"Error happened on basicDefense: {_e}");
            return false;
        }
    }
}