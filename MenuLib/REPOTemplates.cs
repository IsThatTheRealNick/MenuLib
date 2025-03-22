using UnityEngine;

namespace MenuLib;

internal static class REPOTemplates
{
    internal static readonly RectTransform pageDimmerTemplate;
    internal static readonly RectTransform simplePageTemplate;
    internal static readonly RectTransform buttonTemplate;
    internal static readonly RectTransform popupPageTemplate;
    internal static readonly RectTransform toggleTemplate;
    internal static readonly RectTransform sliderTemplate;
    internal static readonly RectTransform keybindTemplate;
    internal static readonly RectTransform labelTemplate;

    static REPOTemplates()
    {
        var menuPages = MenuManager.instance.menuPages;

        foreach (var menuPageData in menuPages)
        {
            var menuPageTransform = menuPageData.menuPage.transform;
            
            switch (menuPageData.menuPageIndex)
            {
                case MenuPageIndex.Main:
                {
                    simplePageTemplate = (RectTransform) menuPageTransform;
                    buttonTemplate = (RectTransform) simplePageTemplate.Find("Menu Button - Quit game");
                    break;
                }
                case MenuPageIndex.Settings:
                {
                    pageDimmerTemplate = (RectTransform) menuPageTransform.GetChild(0);
                    break;
                }
                case MenuPageIndex.SettingsGraphics:
                {
                    popupPageTemplate = (RectTransform) menuPageTransform;
                    break;
                }
                case MenuPageIndex.SettingsAudio:
                {
                    var scroller = menuPageTransform.Find("Menu Scroll Box/Mask/Scroller");
                    toggleTemplate = (RectTransform) scroller.Find("Bool Setting - Push to Talk");
                    sliderTemplate = (RectTransform) scroller.Find("Slider - microphone");
                    break;
                }
                case MenuPageIndex.SettingsControls:
                {
                    var scroller = menuPageTransform.Find("Scroll Box/Mask/Scroller");
                    keybindTemplate = (RectTransform) scroller.Find("Big Button move forward");
                    labelTemplate = (RectTransform) scroller.Find("Header Movement");
                    break;
                }
            }
        }
    }
}