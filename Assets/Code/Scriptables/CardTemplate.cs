using UnityEngine;

public enum CardType { ATTACK = 1, DEFENSE = 2, STATUS = 4 };
public enum CardRange {LINE = 1, SINGLE = 2, CIRCLE = 4, SQUARE = 8, ALL = 16, NONE = 32}
public abstract class CardTemplate : MonoBehaviour {
    public int id;
    
    public string title;
    public string description;
    public int energyCost = -1;
    public int attack = -1;
    public int blockRange = -1;

    public CardType cardType;
    public CardRange cardRange;

    public Sprite cardDecoration;
    public Sprite cardIcon;

    public abstract void onUse(Map _map, Vector2Int _posToUse);
}