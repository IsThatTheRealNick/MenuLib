using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOScrollView : MonoBehaviour
{
    public REPOPopupPage popupPage;

    public float spacing
    {
        get => _spacing;
        set
        {
            if (Math.Abs(_spacing - value) < float.Epsilon)
                return;
            
            _spacing = value;
            UpdateElements();
        }
    }
    
    internal REPOScrollViewElement[] scrollViewElements = [];
    
    private float _spacing;
    
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
        var scroller = menuScrollBox.scroller;
        
        switch (lastElementYPosition)
        {
            case < 0 when !scrollBarGameObject.activeSelf:
                popupPage.scrollBarRectTransform.gameObject.SetActive(true);
                break;
            case >= 0 when scrollBarGameObject.activeSelf:
                popupPage.scrollBarRectTransform.gameObject.SetActive(false);
                scroller.localPosition = scroller.localPosition with { y = 0 };
                break;
        }
        
        REPOReflection.menuScrollBox_ScrollerStartPosition.SetValue(menuScrollBox, Math.Abs(lastElementYPosition / (1f - 0.55f * menuScrollBox.scrollHandle.rect.height / menuScrollBox.scrollBarBackground.rect.height)));
    }
    
    private void OnTransformChildrenChanged() => UpdateElements();
    
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
}