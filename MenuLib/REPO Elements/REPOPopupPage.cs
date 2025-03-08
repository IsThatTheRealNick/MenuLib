using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MenuLib;

public sealed class REPOPopupPage : REPOSimplePage
{
    public Vector2 localPosition { get; private set; } = new(195.64f, 190.6f);
    public bool backgroundDimming { get; private set; }
    
    public Vector2 panelSize { get; private set; } = new(250f, 342f);
    public Padding? maskPadding { get; private set; }

    private RectTransform backgroundPanelTransform, maskTransform, scrollBarTransform, headerTransform;
    private RectTransform hoverAreaTransform, scrollBarOutlineTransform, scrollBarFillTransform, scrollBarBackgroundTransform;
    private Transform pageDimmerTransform, contentTransform;
    
    private MenuScrollBox menuScrollBox;
    
    public REPOPopupPage(string text, Action<REPOPopupPage> setup = null) : base(text, null) => setup?.Invoke(this);

    public void SetLocalPosition(Vector2 newLocalPosition)
    {
        if (backgroundPanelTransform)
            backgroundPanelTransform.localPosition = newLocalPosition;

        UpdateOtherElements();
        
        localPosition = newLocalPosition;
    }
    
    public void SetSize(Vector2 newPanelSize)
    {
        if (backgroundPanelTransform)
            backgroundPanelTransform.sizeDelta = newPanelSize;

        UpdateOtherElements();
        
        panelSize = newPanelSize;
    }

    public void SetBackgroundDimming(bool newBackgroundDimming)
    {
        if (transform)
            switch (newBackgroundDimming)
            {
                case false when pageDimmerTransform:
                    Object.Destroy(pageDimmerTransform.gameObject);
                    break;
                case true when !pageDimmerTransform:
                    pageDimmerTransform = Object.Instantiate(MenuAPI.pageDimmer, transform);
                    pageDimmerTransform.SetAsFirstSibling();
                    break;
            }

        backgroundDimming = newBackgroundDimming;
    }
    
    public void SetMaskPadding(Padding? padding)
    {
        maskPadding = padding;
        UpdateOtherElements();
    }
    
    public void AddElementToScrollView(REPOElement repoElement, Vector2 newPosition)
    {
        initializeButtons += () =>
        {
            var buttonTransform = repoElement.Instantiate();
            
            buttonTransform.SetParent(contentTransform);
            repoElement.SetPosition(newPosition);
            repoElement.afterBeingParented?.Invoke(menuPage);
        };
    }

    public void ClearButtons() => initializeButtons = null;
    
    public override RectTransform GetReference() => MenuAPI.popupPageTemplate;

    public override void SetDefaults()
    {
        menuPage = transform.GetComponent<MenuPage>();
        backgroundPanelTransform = (RectTransform) transform.Find("Panel");
        headerTransform = (RectTransform) transform.Find("Header");
        
        var scrollBoxTransform = transform.Find("Menu Scroll Box");
        
        hoverAreaTransform = (RectTransform) scrollBoxTransform.Find("ScrollBoxHoverArea");
        maskTransform = (RectTransform) scrollBoxTransform.Find("Mask");
        scrollBarTransform = (RectTransform) scrollBoxTransform.Find("Scroll Bar");
        scrollBarOutlineTransform = (RectTransform)  scrollBarTransform.Find("Scroll Bar Bg (1)");
        scrollBarFillTransform = (RectTransform)  scrollBarTransform.Find("Scroll Bar Bg (2)");
        scrollBarBackgroundTransform = (RectTransform)  scrollBarTransform.Find("Scroll Bar Bg");

        transform.name = $"Menu Page {text}";
        
        transform.SetParent(MenuHolder.instance.transform);
        SetLocalPosition(localPosition);

        SetBackgroundDimming(backgroundDimming);

        menuPage.menuPageIndex = (MenuPageIndex)(-1);
        menuPage.disableIntroAnimation = false;
        SetText(text);
        
        backgroundPanelTransform.anchorMax = backgroundPanelTransform.anchorMin = new Vector2(.5f, .5f);
        SetSize(panelSize);
        
        contentTransform = maskTransform.Find("Scroller");
        
        for (var i = contentTransform.childCount - 1; i >= 0; i--)
        {
            var child = contentTransform.GetChild(i);

            if (i < 2)
                continue;
            
            Object.Destroy(child.gameObject);
        }    
        
        initializeButtons?.Invoke();
        
        menuScrollBox = scrollBoxTransform.GetComponent<MenuScrollBox>();
        menuScrollBox.heightPadding = 50;
        
        menuScrollBox.StartCoroutine(ResetScrollBox(menuScrollBox));
        
        transform.gameObject.AddComponent<MenuPageSettings>();
        
        return;

        IEnumerator ResetScrollBox(MenuScrollBox menuScrollBoxInstance)
        {
            yield return null;
            var scrollHandlePosition = scrollBarBackgroundTransform.sizeDelta.y - menuScrollBoxInstance.scrollHandle.sizeDelta.y * .5f;
            menuScrollBoxInstance.scrollHandle.localPosition = menuScrollBoxInstance.scrollHandle.localPosition with { y = scrollHandlePosition }; 
            AccessTools.Field(typeof(MenuScrollBox), "scrollHandleTargetPosition").SetValue(menuScrollBoxInstance, scrollHandlePosition);
            
            AccessTools.Method(typeof(MenuScrollBox), "Start").Invoke(menuScrollBoxInstance, null);
        }
    }

    private void UpdateOtherElements()
    {
        if (!backgroundPanelTransform)
            return;

        var backgroundPanelSize = backgroundPanelTransform.sizeDelta;
        var panelExtents = backgroundPanelSize * .5f;

        if (headerTransform)
            headerTransform.position = backgroundPanelTransform.position + new Vector3(0, panelExtents.y  - headerTransform.sizeDelta.y * .35f, 0);
        
        if (!maskTransform)
            return;
        
        hoverAreaTransform.position = maskTransform.position = backgroundPanelTransform.position - new Vector3(panelExtents.x, panelExtents.y, 0);
        hoverAreaTransform.sizeDelta = maskTransform.sizeDelta = backgroundPanelSize;
                
        var padding = maskPadding ?? new Padding(0, 80, 0, 0);

        if (padding.left != 0)
        {
            maskTransform.sizeDelta = maskTransform.sizeDelta with { x = maskTransform.sizeDelta.x - padding.left };
            maskTransform.position = maskTransform.position with { x = maskTransform.position.x + padding.left };
        }
        
        if (padding.top != 0)
            maskTransform.sizeDelta = maskTransform.sizeDelta with { y = maskTransform.sizeDelta.y - padding.top };
        
        if (padding.right != 0)
            maskTransform.sizeDelta = maskTransform.sizeDelta with { x = maskTransform.sizeDelta.x - padding.right };

        if (padding.bottom != 0)
        {
            maskTransform.sizeDelta = maskTransform.sizeDelta with { y = maskTransform.sizeDelta.y - padding.bottom };
            maskTransform.position = maskTransform.position with { y = maskTransform.position.y + padding.bottom };
        }

        if (!scrollBarTransform)
            return;
        
        var maskSize = maskTransform.sizeDelta;
        scrollBarTransform.sizeDelta = scrollBarTransform.sizeDelta with { y = maskSize.y };
        scrollBarBackgroundTransform.sizeDelta = scrollBarFillTransform.sizeDelta = scrollBarFillTransform.sizeDelta with { y = maskSize.y - 4 };  
        scrollBarOutlineTransform.sizeDelta = scrollBarOutlineTransform.sizeDelta with { y = maskSize.y };
        
        var scrollBarXExtent = scrollBarTransform.sizeDelta.x * .5f;
        scrollBarTransform.position = maskTransform.position + new Vector3(maskSize.x - scrollBarXExtent, 0, 0);
        hoverAreaTransform.sizeDelta = hoverAreaTransform.sizeDelta with { x = hoverAreaTransform.sizeDelta.x + scrollBarXExtent + 4 };
        
    }
}