using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOScrollView : MonoBehaviour
{
    private REPOPopupPage popupPage;
    
    public void UpdateElements()
    {
        var maskSizeDeltaY = popupPage.maskRectTransform.sizeDelta.y;
        
        var lastElementYPosition = 0f;
        var yPosition = maskSizeDeltaY;
        
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

        var scrollHeight = Math.Abs(lastElementYPosition);

        var scrollBarGameObject = popupPage.scrollBarRectTransform.gameObject;
        if (scrollHeight > maskSizeDeltaY && !scrollBarGameObject.activeSelf)
        {
            scrollBarGameObject.SetActive(true);
            REPOReflection.menuPage_ScrollBoxes.SetValue(popupPage.menuPage, (int) REPOReflection.menuPage_ScrollBoxes.GetValue(popupPage.menuPage) + 1);
        } else if (scrollHeight <= maskSizeDeltaY && scrollBarGameObject.activeSelf)
        {
            scrollBarGameObject.SetActive(false);
            REPOReflection.menuPage_ScrollBoxes.SetValue(popupPage.menuPage, (int) REPOReflection.menuPage_ScrollBoxes.GetValue(popupPage.menuPage) - 1);
        }
        
        var minScroll = popupPage.menuScrollBox.scrollHandle.rect.height * .5f / popupPage.menuScrollBox.scrollBarBackground.rect.height * 1.1f;
        REPOReflection.menuScrollBox_scrollerStartPosition.SetValue(popupPage.menuScrollBox, scrollHeight/(1f - minScroll));
    }

    private void Awake() => popupPage = GetComponentInParent<REPOPopupPage>();

    private void OnTransformChildrenChanged() => UpdateElements();
}