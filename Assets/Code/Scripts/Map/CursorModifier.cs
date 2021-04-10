using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CursorModifier : MonoBehaviour {

    [SerializeField] private Texture2D mouseUp;
    [SerializeField] private Texture2D mouseDown;
    [SerializeField] private Texture2D mouseGrabbed;

    private void Start() {
        Cursor.SetCursor(mouseUp, Vector2.zero, CursorMode.Auto);
    }

    private void Update() {
        if (Input.GetMouseButtonDown((int) MouseButton.LeftMouse))
            Cursor.SetCursor(mouseDown, Vector2.zero, CursorMode.Auto);

        if (Input.GetMouseButtonDown((int) MouseButton.RightMouse))
            Cursor.SetCursor(mouseGrabbed, Vector2.zero, CursorMode.Auto);

        if(Input.GetMouseButtonUp((int) MouseButton.RightMouse) || Input.GetMouseButtonUp((int) MouseButton.LeftMouse))
            Cursor.SetCursor(mouseUp, Vector2.zero, CursorMode.Auto);
    }
}
