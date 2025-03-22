using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOScrollView : MonoBehaviour
{
    public REPOPopupPage popupPage;

    public float spacing;

    private REPOScrollViewElement[] scrollViewElements = [];
    
    public void UpdateElements()
    {
        scrollViewElements = GetComponentsInChildren<REPOScrollViewElement>(true);
        
        var lastElementYPosition = 0f;
        var yPosition = popupPage.maskRectTransform.sizeDelta.y;

        foreach (var scrollViewElement in scrollViewElements)
        {
            if (!scrollViewElement.visibility)
                continue;
            
            var localPosition = scrollViewElement.rectTransform.localPosition;

            if (scrollViewElement.topPadding is { } topPadding)
                yPosition -= topPadding;
            
            yPosition -= scrollViewElement.rectTransform.rect.height;
            lastElementYPosition = localPosition.y = yPosition;
            
            if (scrollViewElement.bottomPadding is { } bottomPadding)
                yPosition -= bottomPadding;

            yPosition -= spacing;

            scrollViewElement.rectTransform.localPosition = localPosition;
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

    private void Update()
    {
        var maskRectTransform = popupPage.maskRectTransform;

        foreach (var scrollViewElement in scrollViewElements)
        {
            if (!scrollViewElement.visibility)
                continue;
            
            var elementPosition = scrollViewElement.transform.position;
            var isInRange = elementPosition.y <= maskRectTransform.position.y + maskRectTransform.sizeDelta.y + 50 && elementPosition.y >= maskRectTransform.position.y - 50;

            var elementGameObject = scrollViewElement.gameObject; 
            
            switch (isInRange)
            {
                case false when elementGameObject.activeSelf:
                    elementGameObject.SetActive(false);
                    break;
                case true when !elementGameObject.activeSelf:
                    elementGameObject.SetActive(true);
                    break;
            }   
        }
    }

    private void OnTransformChildrenChanged() => UpdateElements();
}