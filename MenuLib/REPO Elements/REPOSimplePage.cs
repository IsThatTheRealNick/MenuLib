using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MenuLib;

public class REPOSimplePage : REPOElement
{
    public string text { get; private set; }

    internal MenuPage menuPage;
    
    internal Action initializeButtons;

    public REPOSimplePage(string text, Action<REPOSimplePage> onSetup)
    {
        this.text = text;
        onSetup?.Invoke(this);
    }

    public void SetText(string newText)
    {
        if (menuPage?.menuHeader)
            menuPage.menuHeader.text = newText;
        
        text = newText;
    }
    
    public void AddElementToPage(REPOElement repoElement, Vector2 newPosition)
    {
        initializeButtons += () =>
        {
            var buttonTransform = repoElement.Instantiate();
            
            buttonTransform.SetParent(transform);
            repoElement.SetPosition(newPosition);
            repoElement.afterBeingParented?.Invoke(menuPage);
        };
    }

    public void OpenPage(bool addOnTop)
    {
        Instantiate();
        
        if (addOnTop)
            OpenPageOnTop();
        else 
            OpenPageNormal();
    }

    public void ClosePage(bool closePagesAddedOnTop)
    {
        if (closePagesAddedOnTop)
            MenuManager.instance.PageCloseAllAddedOnTop();
        
        menuPage.PageStateSet(MenuPage.PageState.Closing);

        var parentPage = AccessTools.Field(typeof(MenuPage), "pageUnderThisPage").GetValue(menuPage) as MenuPage;
        
        if (parentPage)
            MenuManager.instance.PageSetCurrent(parentPage.menuPageIndex, parentPage);
    }
    
    public override RectTransform GetReference() => MenuAPI.simplePageTemplate;
    
    public override void SetDefaults()
    {
        menuPage = transform.GetComponent<MenuPage>();
        
        transform.name = $"Menu Page {text}";
        
        transform.SetParent(MenuHolder.instance.transform);

        menuPage.menuPageIndex = (MenuPageIndex)(-1);
        menuPage.disableIntroAnimation = false;
        SetText(text);

        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            
            if (!child.name.Contains("Menu Button"))
                continue;
            
            Object.Destroy(child.gameObject);
        }    
        
        Object.Destroy(transform.GetComponent<MenuPageMain>());
        
        initializeButtons?.Invoke();
    }

    private void OpenPageNormal()
    {
        var currentMenuPage = AccessTools.Field(typeof(MenuManager), "currentMenuPage").GetValue(MenuManager.instance) as MenuPage;
        AccessTools.Method(typeof(MenuManager), "PageInactiveAdd").Invoke(MenuManager.instance, [ currentMenuPage ]);
        currentMenuPage?.PageStateSet(MenuPage.PageState.Inactive);   
        
        menuPage.transform.localPosition = Vector3.zero;
        MenuManager.instance.PageAdd(menuPage);
        menuPage.StartCoroutine(AccessTools.Method(typeof(MenuPage), "LateStart").Invoke(menuPage, null) as IEnumerator);
        
        AccessTools.Field(typeof(MenuPage), "addedPageOnTop").SetValue(menuPage, false);

        MenuManager.instance.PageSetCurrent(menuPage.menuPageIndex, menuPage);
        
        AccessTools.Field(typeof(MenuPage), "pageIsOnTopOfOtherPage").SetValue(menuPage, true);
        AccessTools.Field(typeof(MenuPage), "pageUnderThisPage").SetValue(menuPage, currentMenuPage);
    }

    private void OpenPageOnTop()
    {
        var currentMenuPage = AccessTools.Field(typeof(MenuManager), "currentMenuPage").GetValue(MenuManager.instance) as MenuPage;
        
        if (AccessTools.Field(typeof(MenuManager), "addedPagesOnTop").GetValue(MenuManager.instance) is not List<MenuPage> addedPagesOnTop || addedPagesOnTop.Contains(currentMenuPage))
            return;
        
        menuPage.transform.localPosition = Vector3.zero;
        MenuManager.instance.PageAdd(menuPage);
        menuPage.StartCoroutine(AccessTools.Method(typeof(MenuPage), "LateStart").Invoke(menuPage, null) as IEnumerator);
        
        AccessTools.Field(typeof(MenuPage), "addedPageOnTop").SetValue(menuPage, false);
        
        AccessTools.Field(typeof(MenuPage), "parentPage").SetValue(menuPage, currentMenuPage);
        
        if (!addedPagesOnTop.Contains(currentMenuPage))
            addedPagesOnTop.Add(menuPage);
    }
}