using System;
using UnityEngine;

public class TooltipManager : MonoBehaviour {

    private static TooltipManager manager;
    [SerializeField] private Tooltip tooltip;

    private void Awake() {
        manager = this;
        LeanTween.alpha(manager.tooltip.gameObject.GetComponent<RectTransform>(), 0f, 0.1f).setEase(LeanTweenType.linear);
    }

    public static void Show(string _header, string _content) {
        manager.tooltip.content.text = _content;
        manager.tooltip.header.text = _header;
        manager.tooltip.resizeTooltip();
        manager.tooltip.Reposition();
        manager.tooltip.gameObject.SetActive(true);
        LeanTween.alpha(manager.tooltip.gameObject.GetComponent<RectTransform>(), 1f, 0.5f).setEase(LeanTweenType.linear);
    }
    
    public static void Hide() {
        manager.tooltip.gameObject.SetActive(false);
        LeanTween.alpha(manager.tooltip.gameObject.GetComponent<RectTransform>(), 0f, 0.1f).setEase(LeanTweenType.linear);
    }
}