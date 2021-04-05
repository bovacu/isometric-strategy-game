using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private Image leftSide;
    [SerializeField] private Image rightSide;

    private Vector2 gridPos;
    public Vector2 gridPosition {
        get => gridPos;

        set {
            positionText.text = $"({value.x}, {value.y})";
            gridPos = value;
        }
    }

    public void activateLeftSide(bool _activate) {
        leftSide.gameObject.SetActive(_activate);
    }
    
    public void activateRightSide(bool _activate) {
        rightSide.gameObject.SetActive(_activate);
    }
    
    public Vector2 size { get; set; }
}