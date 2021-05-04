using System.Collections.Generic;
using UnityEngine;

public class BinProp : Prop {

    [SerializeField] private SpriteRenderer sprite;
    private Vector2 currentCell;

    public PropInfoJson jsonInfo;

    public override PropInfoJson getJsonInfo() {
        return jsonInfo;
    }

    public override void setJsonInfo(PropInfoJson _jsonInfo) {
        jsonInfo = _jsonInfo;
    }

    public override void setLayer(int _layer) {
        sprite.sortingOrder = _layer;
    }
    
    public override Vector2 getCell() {
        return currentCell;
    }

    public override void setCell(Vector2 _finalCell) {
        currentCell = _finalCell;
    }

    public override bool isDestructible() {
        return jsonInfo.destructible.canDestroy;
    }

    public override void setDestructible(bool _destructible) {
        jsonInfo.destructible.canDestroy = _destructible;
    }

    public override bool isMovable() {
        return jsonInfo.movable.canMove;
    }

    public override void setMovable(bool _movable) {
        jsonInfo.movable.canMove = _movable;
    }

    public override List<LootItem> getLoot() {
        return null;
    }

    public override void setLoot(List<LootItem> _lootItems) {
        throw new System.NotImplementedException();
    }

    public override void breakAnim(Vector2 _finalPos, bool _inmediate = false) {
        throw new System.NotImplementedException();
    }

    public override void moveAnim(Vector2 _finalPos, bool _inmediate = false) {
        throw new System.NotImplementedException();
    }
}