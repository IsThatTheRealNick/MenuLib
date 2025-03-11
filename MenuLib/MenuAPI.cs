using System;
using UnityEngine;
using UnityEngine.Events;

namespace MenuLib;

public static class MenuAPI
{
    internal static RectTransform pageDimmer { get; private set; }
    internal static RectTransform simplePageTemplate { get; private set; }
    internal static RectTransform buttonTemplate { get; private set; }
    internal static RectTransform popupPageTemplate { get; private set; }
    internal static RectTransform toggleTemplate { get; private set; }
    internal static RectTransform sliderTemplate { get; private set; }
    internal static RectTransform keybindTemplate { get; private set; }
    
    internal static Action<MenuPageMain> addToMainMenuEvent;
    internal static Action<MenuPageEsc> addToEscapeMenuEvent;

    private static bool initialized;

    private static MenuButtonPopUp menuButtonPopup;

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

    internal static void Initialize()
    {
        if (initialized)
            return;
        
        var menuPages = MenuManager.instance.menuPages;

        foreach (var menuPageData in menuPages)
        {
            var menuPageTransform = menuPageData.menuPage.transform;
            
            switch (menuPageData.menuPageIndex)
            {
                case MenuPageIndex.Main:
                    simplePageTemplate = (RectTransform) menuPageTransform;
                    buttonTemplate = (RectTransform) simplePageTemplate.Find("Menu Button - Quit game");
                    break;
                case MenuPageIndex.Settings:
                    pageDimmer = (RectTransform) menuPageTransform.GetChild(0);
                    break;
                case MenuPageIndex.SettingsGraphics:
                    popupPageTemplate = (RectTransform) menuPageTransform;
                    break;
                case MenuPageIndex.SettingsAudio:
                    toggleTemplate = (RectTransform) menuPageTransform.Find("Menu Scroll Box/Mask/Scroller/Bool Setting - Push to Talk");
                    sliderTemplate = (RectTransform) menuPageTransform.Find("Menu Scroll Box/Mask/Scroller/Slider - microphone");
                    break;
                case MenuPageIndex.SettingsControls:
                    keybindTemplate = (RectTransform) menuPageTransform.Find("Scroll Box/Mask/Scroller/Big Button move forward");
                    break;
            }
        }

        initialized = true;
    }
}