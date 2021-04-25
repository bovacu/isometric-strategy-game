using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Cell : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] public GameObject upSide;

    public MapCellJson mapCellJson;

    public void setDebugText(Vector2 _pos) {
        positionText.text = $"({_pos.x}, {_pos.y})";
    }
    
    public Vector2 size { get; set; }

    public abstract void update(RoomManager _roomManager);

    public abstract void interact(Target _target);
    
    public void setLayer(int _layer) {
        upSide.GetComponent<SpriteRenderer>().sortingOrder = _layer;
        setLayerForInnerComponents(_layer);
    }

    protected virtual void setLayerForInnerComponents(int _layer) {
        
    }
}