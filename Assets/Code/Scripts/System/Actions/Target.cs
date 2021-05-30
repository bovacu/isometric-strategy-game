using UnityEngine;

public interface Target {

    void setHealth(int _health);
    void setEnergy(int _energy);
    void setStatus(StatusType _status);
    void setCell(Vector2 _cell);

    void setMeleeAttack(int _meleeAttack);
    int getMeleeAttack();
    
    Vector2 getCell();
    int getHealth();
    int getEnergy();
    StatusType getStatus();

    void moveAnim(Vector2 _finalPos, bool _inmediate = false);
    void meleeAnim(Vector2 _finalPos, bool _inmediate = false);
    void rangeAnim(Vector2 _finalPos, bool _inmediate = false);
    void defenseAnim(Vector2 _finalPos, bool _inmediate = false);

    void updateHealthStatus();
}