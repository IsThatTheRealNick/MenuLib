using System;
using System.Collections;
using HarmonyLib;
using MenuLib.MonoBehaviors;
using UnityEngine;
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

    private REPOScrollBoxVisibilityManager scrollBoxVisibilityManager;
    
    public REPOPopupPage(string text, Action<REPOPopupPage> setup = null) : base(text, null) => setup?.Invoke(this);

    public new REPOPopupPage SetPosition(Vector2 newLocalPosition)
    {
        if (backgroundPanelTransform)
            backgroundPanelTransform.localPosition = newLocalPosition;

        UpdateOtherElements();
        
        localPosition = newLocalPosition;
        return this;
    }
    
    public REPOPopupPage SetSize(Vector2 newPanelSize)
    {
        if (backgroundPanelTransform)
            backgroundPanelTransform.sizeDelta = newPanelSize;

        UpdateOtherElements();
        
        panelSize = newPanelSize;
        return this;
    }

    public REPOPopupPage SetBackgroundDimming(bool newBackgroundDimming)
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
        return this;
    }
    
    public REPOPopupPage SetMaskPadding(Padding? padding)
    {
        maskPadding = padding;
        UpdateOtherElements();
        return this;
    }
    
    public REPOPopupPage AddElementToScrollView(REPOElement repoElement, Vector2 newPosition)
    {
        initializeButtons += () => {
            var buttonTransform = repoElement.Instantiate();
            
            buttonTransform.SetParent(contentTransform);
            repoElement.SetPosition(newPosition);
            repoElement.afterBeingParented?.Invoke(menuPage);
        };
        return this;
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
        menuScrollBox = scrollBoxTransform.GetComponent<MenuScrollBox>();
        contentTransform = maskTransform.Find("Scroller");

        scrollBoxVisibilityManager = transform.gameObject.AddComponent<REPOScrollBoxVisibilityManager>();
        scrollBoxVisibilityManager.scroller = contentTransform as RectTransform;
        scrollBoxVisibilityManager.mask = maskTransform;
        
        transform.name = $"Menu Page {text}";
        
        transform.SetParent(MenuHolder.instance.transform);
        SetPosition(localPosition);

        SetBackgroundDimming(backgroundDimming);

        menuPage.menuPageIndex = (MenuPageIndex)(-1);
        menuPage.disableIntroAnimation = false;
        SetText(text);
        
        backgroundPanelTransform.anchorMax = backgroundPanelTransform.anchorMin = new Vector2(.5f, .5f);
        SetSize(panelSize);
        
        for (var i = contentTransform.childCount - 1; i >= 0; i--)
        {
            var child = contentTransform.GetChild(i);

            if (i < 2)
                continue;
            
            Object.Destroy(child.gameObject);
        }
        
        initializeButtons?.Invoke();
        
        menuScrollBox.StartCoroutine(ResetScrollBox());
        
        transform.gameObject.AddComponent<MenuPageSettings>();
        
        return;

        IEnumerator ResetScrollBox()
        {
            yield return null;
            
            /*AccessTools.Field(typeof(MenuScrollBox), "parentPage").SetValue(menuScrollBox, menuPage);
            AccessTools.Field(typeof(MenuScrollBox), "scrollHandleTargetPosition").SetValue(menuScrollBox, scrollHandlePosition);*/
            
            var scrollHandlePosition = scrollBarBackgroundTransform.sizeDelta.y - menuScrollBox.scrollHandle.sizeDelta.y * .5f;
            menuScrollBox.scrollHandle.localPosition = menuScrollBox.scrollHandle.localPosition with { y = scrollHandlePosition }; 
            AccessTools.Field(typeof(MenuScrollBox), "scrollHandleTargetPosition").SetValue(menuScrollBox, scrollHandlePosition);
            
            
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            for (var i = 0; i < contentTransform.childCount; i++)
            {
                if (i <= 2)
                    continue;
                
                var child = contentTransform.GetChild(i) as RectTransform;
                
                var corners = new Vector3[4];
                child!.GetWorldCorners(corners);
                
                for (var j = 0; j < 4; j++)
                    corners[j] = contentTransform.InverseTransformPoint(corners[j]);
                
                var childMinY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
                var childMaxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        
                minY = Mathf.Min(minY, childMinY);
                maxY = Mathf.Max(maxY, childMaxY);
            }

            var height = Math.Abs(maxY - minY);
            
            AccessTools.Field(typeof(MenuScrollBox), "scrollHeight").SetValue(menuScrollBox, height);
            AccessTools.Field(typeof(MenuScrollBox), "scrollerStartPosition").SetValue(menuScrollBox, height + 42f);
            AccessTools.Field(typeof(MenuScrollBox), "scrollerEndPosition").SetValue(menuScrollBox, contentTransform.localPosition.y);

            if (height < maskTransform.sizeDelta.y)
                menuScrollBox.scrollBar.SetActive(false);
            else
            {
                menuScrollBox.scrollBar.SetActive(true);
                
                var scrollBoxesFieldInfo = AccessTools.Field(typeof(MenuPage), "scrollBoxes"); 
                scrollBoxesFieldInfo.SetValue(menuPage, (int) scrollBoxesFieldInfo.GetValue(menuPage) + 1);
            }
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