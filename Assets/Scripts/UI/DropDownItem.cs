using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class needs to be on the Item GameObject of a drop down. It's a hack so the scrollbar follows the dropdown
/// </summary>
public class DropDownItem : MonoBehaviour, ISelectHandler {
    private ScrollRect _scrollRect;
    private float _scrollPosition = 1f;

    private void Start() {
        _scrollRect = GetComponentInParent<ScrollRect>(true);
        int childCount = _scrollRect.content.transform.childCount - 1;
        int childIndex = transform.GetSiblingIndex();

        childIndex = childIndex < (childCount / 2f) ? childIndex - 1 : childIndex;
        _scrollPosition = 1 - childIndex / (float)childCount;
    }

    public void OnSelect(BaseEventData eventData) {
        if (_scrollRect != null) {
            _scrollRect.verticalScrollbar.value = _scrollPosition;
        }
    }
}