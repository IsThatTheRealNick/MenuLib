using System.Reflection;
using HarmonyLib;

namespace MenuLib;

public static class REPOReflection
{
    public static readonly FieldInfo menuManager_CurrentMenuPage = AccessTools.Field(typeof(MenuManager), "currentMenuPage");
    public static readonly FieldInfo menuManager_AddedPagesOnTop = AccessTools.Field(typeof(MenuManager), "addedPagesOnTop");
    public static readonly FieldInfo menuPage_AddedPageOnTop = AccessTools.Field(typeof(MenuPage), "addedPageOnTop");
    public static readonly FieldInfo menuPage_PageUnderThisPage = AccessTools.Field(typeof(MenuPage), "pageUnderThisPage");
    public static readonly FieldInfo menuPage_ParentPage = AccessTools.Field(typeof(MenuPage), "parentPage");
    public static readonly FieldInfo menuPage_PageIsOnTopOfOtherPage = AccessTools.Field(typeof(MenuPage), "pageIsOnTopOfOtherPage");
    public static readonly FieldInfo menuPage_ScrollBoxes = AccessTools.Field(typeof(MenuPage), "scrollBoxes");
    public static readonly FieldInfo menuPage_CurrentPageState = AccessTools.Field(typeof(MenuPage), "currentPageState");
    public static readonly FieldInfo menuPage_AnimateAwayPosition = AccessTools.Field(typeof(MenuPage), "animateAwayPosition");
    public static readonly FieldInfo menuButton_ParentPage = AccessTools.Field(typeof(MenuButton), "parentPage");
    public static readonly FieldInfo menuScrollBox_scrollerEndPosition = AccessTools.Field(typeof(MenuScrollBox), "scrollerEndPosition");
    public static readonly FieldInfo menuScrollBox_scrollerStartPosition = AccessTools.Field(typeof(MenuScrollBox), "scrollerStartPosition");
    public static readonly FieldInfo menuSelectableElement_menuID = AccessTools.Field(typeof(MenuSelectableElement), "menuID");
    public static readonly MethodInfo menuManager_PageInactiveAdd = AccessTools.Method(typeof(MenuManager), "PageInactiveAdd");
    public static readonly MethodInfo menuPage_LateStart = AccessTools.Method(typeof(MenuPage), "LateStart");
}