using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] public GameObject upSide;
    [SerializeField] public GameObject leftSide;
    [SerializeField] public GameObject rightSide;

    private Vector2 gridPos;
    public Vector2 gridPosition {
        get => gridPos;

        set {
            positionText.text = $"({value.x}, {value.y})";
            gridPos = value;
        }
    }

    public void activateLeftSide(bool _activate) {
        leftSide.SetActive(_activate);
    }
    
    public void activateRightSide(bool _activate) {
        rightSide.SetActive(_activate);
    }
    
    public Vector2 size { get; set; }
}