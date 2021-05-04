using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : MonoBehaviour {

    public abstract PropInfoJson getJsonInfo();
    public abstract void setJsonInfo(PropInfoJson _jsonInfo);
    
    public abstract void setLayer(int _layer);
    
    public abstract Vector2 getCell();
    public abstract void setCell(Vector2 _finalCell);

    public abstract bool isDestructible();
    public abstract void setDestructible(bool _destructible);

    public abstract bool isMovable();
    public abstract void setMovable(bool _movable);

    public abstract List<LootItem> getLoot();
    public abstract void setLoot(List<LootItem> _lootItems);

    public abstract void breakAnim(Vector2 _finalPos, bool _inmediate = false);
    public abstract void moveAnim(Vector2 _finalPos, bool _inmediate = false);
}