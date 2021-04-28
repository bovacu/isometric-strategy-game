using UnityEngine;

public abstract class BaseKey : MonoBehaviour, AI {

    protected Vector2 currentCell;
    protected EnemyInfoJson enemyInfoJson;

    protected int currentHealth;
    protected int currentAttack;
    protected int currentEnergy;
    protected int currentDefense;

    protected StatusType currentStatus;
    
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

    public NextAction loadNextAction(RoomManager _roomManager) {
        return NextAction.MOVE;
    }
    
    public Vector2 loadFinalCell(RoomManager _roomManager) {
        return _roomManager.availableCells[IsoMath.randomInt(0, _roomManager.availableCells.Count - 1)];
    }
    
    public abstract void moveAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void meleeAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void rangeAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void defenseAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void updateHealthStatus();
}