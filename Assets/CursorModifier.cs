using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CursorModifier : MonoBehaviour {

    [SerializeField] private Sprite mouseUp;
    [SerializeField] private Sprite mouseDown;
    [SerializeField] private Sprite mouseGrabbed;

    private void Start() {
        Cursor.SetCursor(mouseUp.texture, Vector2.zero, CursorMode.Auto);
    }

    private void Update() {
        Debug.Log(Input.mousePosition);
        
        if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
            Cursor.SetCursor(mouseDown.texture, Vector2.zero, CursorMode.Auto);

        if (Input.GetMouseButtonDown((int) MouseButton.RightMouse))
            Cursor.SetCursor(mouseGrabbed.texture, Vector2.zero, CursorMode.Auto);

        if(Input.GetMouseButtonUp((int) MouseButton.RightMouse) || Input.GetMouseButtonUp((int) MouseButton.LeftMouse))
            Cursor.SetCursor(mouseUp.texture, Vector2.zero, CursorMode.Auto);
    }
}
