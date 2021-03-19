using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {
    
    public Vector2 gridPosition { get; set; }
    public Image originalSprite { get; set; }
    public string tileId { get; set; }
    
    public Vector2 size { get; set; }

    public bool mouseIsOver(float _x, float _y) {
        return false;
    }

    public void setSelected(bool _selected) {
        if(_selected) select();
        else deselect();
    }

    private void select() {
        originalSprite.color = Color.red;
    }

    private void deselect() {
        originalSprite.color = Color.red;
    }
}