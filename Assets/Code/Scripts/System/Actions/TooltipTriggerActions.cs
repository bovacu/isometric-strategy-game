using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriggerActions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private LTDescr delay;
    [SerializeField] private int actionId;
    private bool abortShow = false;

    public void OnPointerEnter(PointerEventData eventData) {
        delay = LeanTween.delayedCall(.5f, () => {
            var _action = GameConfig.basicMovements[actionId];
            TooltipManager.Show(Locize.translateHashtags(_action.name), Locize.translateHashtags(_action.description));  
        });
    }

    public void OnPointerExit(PointerEventData eventData) {
        LeanTween.cancel(delay.uniqueId);
        TooltipManager.Hide();
    }
}