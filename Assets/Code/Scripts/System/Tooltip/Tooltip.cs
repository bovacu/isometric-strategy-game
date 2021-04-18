using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour {
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;
    public LayoutElement layoutElement;
    public int characterWrapLimit = 70;

    public void resizeTooltip() {
        layoutElement.enabled = header.text.Length > characterWrapLimit || content.text.Length > characterWrapLimit;   
    }

    public void Reposition() {
        Vector2 _pos = Input.mousePosition;
        var _rect = GetComponent<RectTransform>();
        var sizeDelta = _rect.sizeDelta;
        var _extra = _pos.x > Screen.width / 2f ? sizeDelta.x : 0;
        _rect.anchoredPosition = new Vector2(_pos.x - Screen.width / 2f + sizeDelta.x / 2f - _extra, _pos.y - Screen.height / 2f + sizeDelta.y / 2f);
    }

    private void Update() {
        if (gameObject.activeInHierarchy) {
            Reposition();
        }
    }
}