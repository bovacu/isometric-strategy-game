using System.Collections.Generic;
using UnityEngine;

public class BinProp : MonoBehaviour, Prop {

    [SerializeField] private SpriteRenderer sprite;
    private Vector2 currentCell;

    public PropInfoJson jsonInfo;

    public PropInfoJson getJsonInfo() {
        return jsonInfo;
    }

    public void setJsonInfo(PropInfoJson _jsonInfo) {
        jsonInfo = _jsonInfo;
    }

    public void setLayer(int _layer) {
        sprite.sortingOrder = _layer;
    }
    
    public Vector2 getCell() {
        return currentCell;
    }

    public void setCell(Vector2 _finalCell) {
        currentCell = _finalCell;
    }

    public bool isDestructible() {
        return jsonInfo.destructible.canDestroy;
    }

    public void setDestructible(bool _destructible) {
        jsonInfo.destructible.canDestroy = _destructible;
    }

    public bool isMovable() {
        return jsonInfo.movable.canMove;
    }

    public void setMovable(bool _movable) {
        jsonInfo.movable.canMove = _movable;
    }

    public List<LootItem> getLoot() {
        return null;
    }

    public void setLoot(List<LootItem> _lootItems) {
        throw new System.NotImplementedException();
    }

    public void breakAnim(Vector2 _finalPos, bool _inmediate = false) {
        throw new System.NotImplementedException();
    }

    public void moveAnim(Vector2 _finalPos, bool _inmediate = false) {
        throw new System.NotImplementedException();
    }
}