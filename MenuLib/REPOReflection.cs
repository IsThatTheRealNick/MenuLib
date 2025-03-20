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
    public static readonly FieldInfo menuButton_ParentPage = AccessTools.Field(typeof(MenuButton), "parentPage");
    public static readonly MethodInfo menuManager_PageInactiveAdd = AccessTools.Method(typeof(MenuManager), "PageInactiveAdd");
    public static readonly MethodInfo menuPage_LateStart = AccessTools.Method(typeof(MenuPage), "LateStart");
}