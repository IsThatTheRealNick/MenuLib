using System;
using System.Collections;
using System.Collections.Generic;
using MenuLib.MonoBehaviors;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MenuLib;

public static class MenuAPI
{
    internal static BuilderDelegate mainMenuBuilderDelegates;
    internal static BuilderDelegate escapeMenuBuilderDelegates;

    private static MenuButtonPopUp menuButtonPopup;
    
    public delegate void BuilderDelegate(Transform parent);
    
    public static void AddElementToMainMenu(BuilderDelegate builderDelegate) => mainMenuBuilderDelegates += builderDelegate;
    
    public static void AddElementToEscapeMenu(BuilderDelegate builderDelegate) => escapeMenuBuilderDelegates += builderDelegate;

    public static void CloseAllPagesAddedOnTop() => MenuManager.instance.PageCloseAllAddedOnTop();
    
#warning Might create custom versions of this
    public static void OpenPopup(string header, Color headerColor, string content, string buttonText, Action onClick) => MenuManager.instance.PagePopUp(header, headerColor, content, buttonText);

    public static void OpenPopup(string header, Color headerColor, string content, string leftButtonText, Action onLeftClicked, string rightButtonText, Action onRightClicked = null)
    {
        if (!menuButtonPopup)
            menuButtonPopup = MenuManager.instance.gameObject.AddComponent<MenuButtonPopUp>();

        menuButtonPopup.option1Event = new UnityEvent();
        menuButtonPopup.option2Event = new UnityEvent();
        
        if (onLeftClicked != null)
            menuButtonPopup.option1Event.AddListener(new UnityAction(onLeftClicked));
        
        if (onRightClicked != null)
            menuButtonPopup.option2Event.AddListener(new UnityAction(onRightClicked));
        
        MenuManager.instance.PagePopUpTwoOptions(menuButtonPopup, header, headerColor, content, leftButtonText, rightButtonText);
    }
    
    public static REPOButton CreateREPOButton(string text, Action onClick, Transform parent, Vector2 localPosition = default)
    {
        var newRectTransform = Object.Instantiate(REPOTemplates.buttonTemplate, parent);
        newRectTransform.name = $"Menu Button - {text}";

        newRectTransform.localPosition = localPosition;
        
        var repoButton = newRectTransform.gameObject.AddComponent<REPOButton>();

        repoButton.labelTMP.text = text;
        
        if (onClick != null)
            repoButton.button.onClick.AddListener(new UnityAction(onClick));
        
        return repoButton;
    }
    
    public static REPOToggle CreateREPOToggle(string text, Action<bool> onToggle, Transform parent, Vector2 localPosition = default, string leftButtonText = "ON", string rightButtonText = "OFF", bool defaultValue = false)
    {
        var newRectTransform = Object.Instantiate(REPOTemplates.toggleTemplate, parent);
        newRectTransform.name = $"Menu Toggle - {text}";

        newRectTransform.localPosition = localPosition;
        
        var repoToggle = newRectTransform.gameObject.AddComponent<REPOToggle>();

        repoToggle.labelTMP.text = text;
        repoToggle.leftButtonTMP.text = leftButtonText;
        repoToggle.rightButtonTMP.text = rightButtonText;
        repoToggle.onToggle = onToggle;
        
        repoToggle.SetState(defaultValue, false);
        return repoToggle;
    }

    public static REPOLabel CreateREPOLabel(string text, Transform parent, Vector2 localPosition = default)
    {
        var newRectTransform = Object.Instantiate(REPOTemplates.labelTemplate, parent);
        newRectTransform.name = $"Label - {text}";

        newRectTransform.localPosition = localPosition;
        
        var repoLabel = newRectTransform.gameObject.AddComponent<REPOLabel>();

        repoLabel.labelTMP.text = text;
        
        return repoLabel;
    }
    
    public static REPOSpacer CreateREPOSpacer(Transform parent, Vector2 localPosition = default, Vector2 size = default)
    {
        var newRectTransform = (RectTransform) new GameObject("Spacer", typeof(RectTransform)).transform;

        newRectTransform.SetParent(parent);
        
        var repoSpacer = newRectTransform.gameObject.AddComponent<REPOSpacer>();

        newRectTransform.localPosition = localPosition;
        newRectTransform.sizeDelta = size;
        
        return repoSpacer;
    }
    
    public static REPOPopupPage CreateREPOPopupPage(string headerText, REPOPopupPage.PresetSide presetSide, bool pageDimmerVisibility = false) => CreatePopupPage(headerText, pageDimmerVisibility, presetSide == REPOPopupPage.PresetSide.Left ? null : new Vector2(40, 0));
    
    public static REPOPopupPage CreatePopupPage(string headerText, bool pageDimmerVisibility = false, Vector2? localPosition = null)
    {
        var newRectTransform = Object.Instantiate(REPOTemplates.popupPageTemplate, MenuHolder.instance.transform);
        newRectTransform.name = $"Menu Page {headerText}";
        
        var repoPopupPage = newRectTransform.gameObject.AddComponent<REPOPopupPage>();
        
        repoPopupPage.rectTransform.localPosition = localPosition ?? new Vector2(-280, 0);
        repoPopupPage.pageDimmerVisibility = pageDimmerVisibility;
        repoPopupPage.headerTMP.text = headerText;
        
        return repoPopupPage;
    }

    internal static void OpenPage(MenuPage menuPage, bool pageOnTop)
    {
        var currentMenuPage = REPOReflection.menuManager_CurrentMenuPage.GetValue(MenuManager.instance) as MenuPage;

        var addedPagesOnTop = REPOReflection.menuManager_AddedPagesOnTop.GetValue(MenuManager.instance) as List<MenuPage>; 
        
        switch (pageOnTop)
        {
            case true when addedPagesOnTop == null || addedPagesOnTop.Contains(currentMenuPage):
                return;
            case false:
                REPOReflection.menuManager_PageInactiveAdd.Invoke(MenuManager.instance, [ currentMenuPage ]);
                currentMenuPage?.PageStateSet(MenuPage.PageState.Inactive);
                break;
        }
        
        menuPage.gameObject.SetActive(true);
        menuPage.transform.localPosition = Vector3.zero;
        MenuManager.instance.PageAdd(menuPage);
        menuPage.StartCoroutine(REPOReflection.menuPage_LateStart.Invoke(menuPage, null) as IEnumerator);
            
        REPOReflection.menuPage_AddedPageOnTop.SetValue(menuPage, false);
        
        if (!pageOnTop)
        {
            MenuManager.instance.PageSetCurrent(menuPage.menuPageIndex, menuPage);
        
            REPOReflection.menuPage_PageIsOnTopOfOtherPage.SetValue(menuPage, true);
            REPOReflection.menuPage_PageUnderThisPage.SetValue(menuPage, currentMenuPage);
            return;
        }
        
        REPOReflection.menuPage_ParentPage.SetValue(menuPage, currentMenuPage);
        addedPagesOnTop.Add(menuPage);
    }
}