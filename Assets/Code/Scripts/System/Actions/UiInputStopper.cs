using UnityEngine;
using UnityEngine.EventSystems;

public class UiInputStopper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public void OnPointerEnter(PointerEventData eventData) {
        MapControls.inputIsAvailable = false;
    }

    public void OnPointerExit(PointerEventData eventData) {
        MapControls.inputIsAvailable = true;
    }
}