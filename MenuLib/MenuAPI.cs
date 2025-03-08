using System;
using UnityEngine;

namespace MenuLib;

public static class MenuAPI
{
    internal static RectTransform pageDimmer { get; private set; }
    internal static RectTransform simplePageTemplate { get; private set; }
    internal static RectTransform buttonTemplate { get; private set; }
    internal static RectTransform popupPageTemplate { get; private set; }
    internal static RectTransform toggleTemplate { get; private set; }
    internal static RectTransform sliderTemplate { get; private set; }
    
    internal static Action<MenuPageMain> addToMainMenuEvent;
    internal static Action<MenuPageEsc> addToEscapeMenuEvent;

    private static bool initialized;

    public static void AddElementToMainMenu(REPOElement repoElement, Vector2 newPosition)
    {
        addToMainMenuEvent += instance =>
        {
            var transform = repoElement.Instantiate();
            
            transform.SetParent(instance.transform);
            repoElement.SetPosition(newPosition);
            repoElement.afterBeingParented?.Invoke(instance.GetComponent<MenuPage>());
        };
    }

    public static void AddElementToEscapeMenu(REPOElement repoElement, Vector2 newPosition)
    {
        addToEscapeMenuEvent += instance => {
            var transform = repoElement.Instantiate();
            
            transform.SetParent(instance.transform);
            repoElement.SetPosition(newPosition);
            repoElement.afterBeingParented?.Invoke(instance.GetComponent<MenuPage>());
        };
    }

    internal static void Initialize()
    {
        if (initialized)
            return;
        
        var menuPages = MenuManager.instance.menuPages;

        foreach (var menuPageData in menuPages)
        {
            switch (menuPageData.menuPageIndex)
            {
                case MenuPageIndex.Main:
                    simplePageTemplate = (RectTransform) menuPageData.menuPage.transform;
                    buttonTemplate = (RectTransform) simplePageTemplate.Find("Menu Button - Quit game");
                    break;
                case MenuPageIndex.Settings:
                    pageDimmer = (RectTransform) menuPageData.menuPage.transform.GetChild(0);
                    break;
                case MenuPageIndex.SettingsGraphics:
                    popupPageTemplate = (RectTransform) menuPageData.menuPage.transform;
                    break;
                case MenuPageIndex.SettingsAudio:
                    toggleTemplate = (RectTransform) menuPageData.menuPage.transform.Find("Menu Scroll Box/Mask/Scroller/Bool Setting - Push to Talk");
                    sliderTemplate = (RectTransform) menuPageData.menuPage.transform.Find("Menu Scroll Box/Mask/Scroller/Slider - microphone");
                    break;
            }
        }

        initialized = true;
    }
}