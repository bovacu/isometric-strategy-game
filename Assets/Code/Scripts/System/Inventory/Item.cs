using UnityEngine;

public enum ItemType { POTION = 1, KEY = 2, BOMB = 4, GOLD = 8 };

public abstract class Item : MonoBehaviour {
    public int id;
    
    public string title;
    public string description;
    public int recovery = -1;
    public int attack = -1;

    public ItemType itemType;
    public CardRange cardRange;
    
    public Sprite itemIcon;

    public abstract void onUse(Map _map, Vector2Int _posToUse);
}