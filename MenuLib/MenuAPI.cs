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
    internal static BuilderDelegate mainMenuBuilderDelegates, escapeMenuBuilderDelegates, lobbyMenuBuilderDelegate;

    private static MenuButtonPopUp menuButtonPopup;
    
    public delegate void BuilderDelegate(Transform parent);
    
    public static void AddElementToMainMenu(BuilderDelegate builderDelegate) => mainMenuBuilderDelegates += builderDelegate;
    
    public static void AddElementToEscapeMenu(BuilderDelegate builderDelegate) => escapeMenuBuilderDelegates += builderDelegate;
    
    public static void AddElementToLobbyMenu(BuilderDelegate builderDelegate) => lobbyMenuBuilderDelegate += builderDelegate;

    public static void CloseAllPagesAddedOnTop() => MenuManager.instance.PageCloseAllAddedOnTop();
    
    public static void OpenPopup(string header, Color headerColor, string content, Action onLeftClicked, Action onRightClicked = null)
    {
        if (!menuButtonPopup)
            menuButtonPopup = MenuManager.instance.gameObject.AddComponent<MenuButtonPopUp>();

        menuButtonPopup.option1Event = new UnityEvent();
        menuButtonPopup.option2Event = new UnityEvent();
        
        if (onLeftClicked != null)
            menuButtonPopup.option1Event.AddListener(new UnityAction(onLeftClicked));
        
        if (onRightClicked != null)
            menuButtonPopup.option2Event.AddListener(new UnityAction(onRightClicked));
        
        //Setting the text in here doesn't work
        MenuManager.instance.PagePopUpTwoOptions(menuButtonPopup, header, headerColor, content, "Yes", "No");
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
    
    public static REPOSlider CreateREPOSlider(string text, string description, Action<float> onValueChanged, Transform parent, Vector2 localPosition = default, float min = 0f, float max = 1f, float precision = 2f, float defaultValue = 0f, string prefix = "", string postfix = "", REPOSlider.BarBehavior barBehavior = REPOSlider.BarBehavior.UpdateWithValue)
    {
        var newRectTransform = Object.Instantiate(REPOTemplates.sliderTemplate, parent);
        newRectTransform.name = $"Float Slider - {text}";

        newRectTransform.localPosition = localPosition;
        
        var repoSlider = newRectTransform.gameObject.AddComponent<REPOSlider>();

        repoSlider.labelTMP.text = text;
        repoSlider.descriptionTMP.text = description; //Add text scroller
        repoSlider.onValueChanged = onValueChanged;
        repoSlider.min = min;
        repoSlider.max = max;
        repoSlider.precision = precision;
        repoSlider.prefix = prefix;
        repoSlider.postfix = postfix;
        repoSlider.barBehavior = barBehavior;
        
        repoSlider.SetValue(defaultValue, false);
        return repoSlider;
    }
    
    public static REPOSlider CreateREPOSlider(string text, string description, Action<int> onValueChanged, Transform parent, Vector2 localPosition = default, int min = 0, int max = 1, int defaultValue = 0, string prefix = "", string postfix = "", REPOSlider.BarBehavior barBehavior = REPOSlider.BarBehavior.UpdateWithValue)
    {
        var newRectTransform = Object.Instantiate(REPOTemplates.sliderTemplate, parent);
        newRectTransform.name = $"Int Slider - {text}";

        newRectTransform.localPosition = localPosition;
        
        var repoSlider = newRectTransform.gameObject.AddComponent<REPOSlider>();

        repoSlider.labelTMP.text = text;
        repoSlider.descriptionTMP.text = description; //Add text scroller
        repoSlider.onValueChanged = f => onValueChanged.Invoke(Convert.ToInt32(f));
        repoSlider.min = min;
        repoSlider.max = max;
        repoSlider.precision = 0;
        repoSlider.prefix = prefix;
        repoSlider.postfix = postfix;
        repoSlider.barBehavior = barBehavior;
        
        repoSlider.SetValue(defaultValue, false);
        return repoSlider;
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