using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOPopupPage : MonoBehaviour
{
    public RectTransform rectTransform;
    public MenuPage menuPage;
    public TextMeshProUGUI headerTMP;

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
    
    private void Awake()
    {
        #warning fix positions, header, scroll, they all have to move together
        rectTransform = transform as RectTransform;
        
        menuPage = GetComponent<MenuPage>();
        headerTMP = GetComponentInChildren<TextMeshProUGUI>();
        
        pageDimmerGameObject = Instantiate(REPOTemplates.pageDimmerTemplate, transform).gameObject;
        pageDimmerGameObject.transform.SetAsFirstSibling();

        menuPage.menuPageIndex = (MenuPageIndex)(-1);
    }
    
    private void Update()
    {
        //Control scroll visibility
    }
}