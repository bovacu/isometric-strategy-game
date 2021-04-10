using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private LTDescr delay;
    [SerializeField] private string id;
    [SerializeField] private string headerForTooltip;
    [SerializeField][Multiline] private string contentForTooltip;
    private bool abortShow = false;

    public void OnPointerEnter(PointerEventData eventData) {
        delay = LeanTween.delayedCall(.5f, () => {
            var _status = GameConfig.status.types.First(_s => _s.id.Equals(id));
            var _content = string.Concat(GameConfig.status.tooltipConfig.effect, _status.effect, "\n", GameConfig.status.tooltipConfig.curedBy, _status.curedBy);
            TooltipManager.Show(Locize.translate(_status.header), Locize.translate(_content));  
        });
    }

    public void OnPointerExit(PointerEventData eventData) {
        LeanTween.cancel(delay.uniqueId);
        TooltipManager.Hide();
    }
}