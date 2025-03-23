using UnityEngine;

namespace MenuLib;

internal static class REPOTemplates
{
    internal static readonly RectTransform pageDimmerTemplate,
        simplePageTemplate,
        buttonTemplate,
        popupPageTemplate,
        toggleTemplate,
        sliderTemplate,
        labelTemplate,
        avatarPreviewTemplate;

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
                    labelTemplate = (RectTransform) menuPageTransform.Find("Scroll Box/Mask/Scroller").Find("Header Movement");
                    break;
                }
                case MenuPageIndex.Escape:
                {
                    avatarPreviewTemplate = (RectTransform) menuPageTransform.Find("Menu Element Player Avatar");
                    break;
                }
            }
        }
    }
}