using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseKey : MonoBehaviour, AI {

    protected Vector2 currentCell;
    protected EnemyInfoJson enemyInfoJson;

    protected int currentHealth;
    protected int currentAttack;
    protected int currentEnergy;
    protected int currentDefense;

    protected StatusType currentStatus;
    protected AIState state;

    public EnemyInfoJson getInfo() {
        return enemyInfoJson;
    }

    public void setInfo(EnemyInfoJson _enemyInfoJson) {
        enemyInfoJson = _enemyInfoJson;

        currentAttack = _enemyInfoJson.baseAttack;
        currentDefense = _enemyInfoJson.baseDefense;
        currentEnergy = _enemyInfoJson.baseEnergy;
        currentHealth = _enemyInfoJson.baseHealth;
    }
    
    public AIState CurrentAIState {
        get => state;
        set => state = value;
    }
    
    public GameObject GameObject {
        get => this.gameObject;
    }

    public void setCell(Vector2 _cell) {
        currentCell = _cell;
    }

    public Vector2 getCell() {
        return currentCell;
    }

    public void setHealth(int _health) {
        currentHealth = IsoMath.ClampI(_health, 0, enemyInfoJson.baseHealth);
    }
    
    public int getHealth() {
        return currentHealth;
    }

    public void setEnergy(int _energy) {
        currentEnergy = IsoMath.ClampI(_energy, 0, enemyInfoJson.baseEnergy);
    }

    public  int getEnergy() {
        return currentEnergy;
    }

    public void setStatus(StatusType _status) {
        currentStatus = _status;
    }

    public StatusType getStatus() {
        return currentStatus;
    }

    public int getMeleeAttack() {
        return currentAttack;
    }

    public void setMeleeAttack(int _meleeAttack) {
        currentAttack = _meleeAttack;
    }

    public IEnumerator startStateMachine(RoomManager _roomManager) {
        Debug.Log("Starting State Machine");
        state = new IdleState(this);
        yield return state.execute(_roomManager);
    }

    public NextAction loadNextAction(RoomManager _roomManager) {
        if(IsoMath.cellDistance(currentCell, _roomManager.getPlayerData().currentCell) > 1)
            return NextAction.MOVE;

        return NextAction.MELEE;
    }
    
    public Vector2 loadFinalCell(RoomManager _roomManager) {
        var _finalCell = _roomManager.availableCells[0];
        
        foreach (var _cell in _roomManager.availableCells) {
            var _newDistance = IsoMath.cellDistance(_cell, _roomManager.getPlayerData().currentCell);
            if (_newDistance < IsoMath.cellDistance(_finalCell, _roomManager.getPlayerData().currentCell))
                _finalCell = _cell;
        }
        
        return _finalCell;
    }
    
    public abstract void moveAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void meleeAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void rangeAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void defenseAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void updateHealthStatus();
}