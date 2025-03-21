using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOPopupPage : MonoBehaviour
{
    public enum PresetSide
    {
        Left,
        Right
    }
    
    public RectTransform rectTransform;
    public MenuPage menuPage;
    public TextMeshProUGUI headerTMP;
    public MenuScrollBox menuScrollBox;
    
    public bool pageDimmerVisibility
    {
        get => pageDimmerGameObject.gameObject.activeSelf;
        set => pageDimmerGameObject.gameObject.SetActive(value);
    }
    
    private GameObject pageDimmerGameObject;

    public void OpenPage(bool openOnTop) => MenuAPI.OpenPage(menuPage, openOnTop);
    
    public void ClosePage(bool closePagesAddedOnTop)
    {
        if (closePagesAddedOnTop)
            MenuManager.instance.PageCloseAllAddedOnTop();
        
        menuPage.PageStateSet(MenuPage.PageState.Closing);

        var parentPage = REPOReflection.menuPage_PageUnderThisPage.GetValue(menuPage) as MenuPage;
        
        if (parentPage)
            MenuManager.instance.PageSetCurrent(parentPage.menuPageIndex, parentPage);
    }
    
    public void AddElement(MenuAPI.BuilderDelegate builderDelegate) => builderDelegate.Invoke(transform);
    
#warning This will have layout settings
    public void AddElementToScrollView(MenuAPI.BuilderDelegate builderDelegate, bool elementIgnoresLayout = false) => builderDelegate.Invoke(menuScrollBox.scroller);
    
    private void Awake()
    {
        menuPage = GetComponent<MenuPage>();
        headerTMP = GetComponentInChildren<TextMeshProUGUI>();
        menuScrollBox = GetComponentInChildren<MenuScrollBox>();
        
        pageDimmerGameObject = Instantiate(REPOTemplates.pageDimmerTemplate, transform).gameObject;
        pageDimmerGameObject.transform.SetAsFirstSibling();

        menuPage.menuPageIndex = (MenuPageIndex) (-1);
     
        rectTransform = (RectTransform) new GameObject("Page Content", typeof(RectTransform)).transform;
        rectTransform.SetParent(transform);
        
        transform.Find("Panel").SetParent(rectTransform);
        headerTMP.transform.parent.SetParent(rectTransform);
        menuScrollBox.transform.SetParent(rectTransform);
        
        Destroy(GetComponent<MenuPageSettingsPage>());
        gameObject.AddComponent<MenuPageSettings>();
    }
    
    private void Update()
    {
        //Control scroll visibility
    }
}