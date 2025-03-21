using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOScrollView : MonoBehaviour
{
    private REPOPopupPage popupPage;
    
    public void UpdateElements()
    {
        var lastElementYPosition = 0f;
        var yPosition = popupPage.maskRectTransform.sizeDelta.y;
        
        for (var i = 2; i < transform.childCount; i++)
        {
            var child = (RectTransform)transform.GetChild(i);

            if (!child.gameObject.activeSelf)
                continue;
            
            var localPosition = child.localPosition;
            
            yPosition -= child.rect.height * (1f - child.pivot.y);
            localPosition.y = yPosition;
            lastElementYPosition = yPosition;

            child.localPosition = localPosition;
        }
        
        popupPage.scrollBarRectTransform.gameObject.SetActive(lastElementYPosition < 0);

        var menuScrollBox = popupPage.menuScrollBox;
        
        var divisor = 1f - menuScrollBox.scrollHandle.rect.height * .5f / menuScrollBox.scrollBarBackground.rect.height * 1.1f;
        REPOReflection.menuScrollBox_scrollerStartPosition.SetValue(menuScrollBox, Math.Abs(lastElementYPosition/divisor));
    }

    private void Awake() => popupPage = GetComponentInParent<REPOPopupPage>();

    private void OnTransformChildrenChanged() => UpdateElements();
}