using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOScrollView : MonoBehaviour
{
    internal REPOPopupPage popupPage;
    
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
        
        var scrollBarGameObject = popupPage.scrollBarRectTransform.gameObject;
        var menuScrollBox = popupPage.menuScrollBox;
        
        switch (lastElementYPosition)
        {
            case < 0 when !scrollBarGameObject.activeSelf:
                popupPage.scrollBarRectTransform.gameObject.SetActive(true);
                break;
            case >= 0 when scrollBarGameObject.activeSelf:
                popupPage.scrollBarRectTransform.gameObject.SetActive(false);
                menuScrollBox.scroller.localPosition = menuScrollBox.scroller.localPosition with { y = 0 };
                break;
        }
        
        var divisor = 1f - menuScrollBox.scrollHandle.rect.height * .5f / menuScrollBox.scrollBarBackground.rect.height * 1.1f;
        REPOReflection.menuScrollBox_scrollerStartPosition.SetValue(menuScrollBox, Math.Abs(lastElementYPosition/divisor));
    }

    private void OnTransformChildrenChanged() => UpdateElements();
}