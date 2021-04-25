using System.Collections.Generic;
using UnityEngine;

public interface Prop {

    PropInfoJson getJsonInfo();
    void setJsonInfo(PropInfoJson _jsonInfo);
    
    void setLayer(int _layer);
    
    Vector2 getCell();
    void setCell(Vector2 _finalCell);

    bool isDestructible();
    void setDestructible(bool _destructible);

    bool isMovable();
    void setMovable(bool _movable);

    List<LootItem> getLoot();
    void setLoot(List<LootItem> _lootItems);

    void breakAnim(Vector2 _finalPos, bool _inmediate = false);
    void moveAnim(Vector2 _finalPos, bool _inmediate = false);
}