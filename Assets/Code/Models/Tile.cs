using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private Image selected;

    private Vector2 gridPos;
    public Vector2 gridPosition {
        get => gridPos;

        set {
            positionText.text = $"({value.x}, {value.y})";
            gridPos = value;
        }
    }

    public Vector2 size { get; set; }

    public void selectTile(bool _select) {
        selected.gameObject.SetActive(_select);
    }
}