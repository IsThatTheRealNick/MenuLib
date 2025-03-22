using MenuLib.Interfaces;
using MenuLib.Structs;
using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOPopupPage : MonoBehaviour, IREPOElement
{
    public enum PresetSide
    {
        Left,
        Right
    }
 
    public delegate RectTransform ScrollViewBuilderDelegate(Transform scrollView);
 
    public RectTransform rectTransform { get; private set; }
    public RectTransform maskRectTransform, scrollBarRectTransform;
    public MenuPage menuPage;
    public TextMeshProUGUI headerTMP;
    public MenuScrollBox menuScrollBox;
    
    public bool pageDimmerVisibility
    {
        get => pageDimmerGameObject.gameObject.activeSelf;
        set => pageDimmerGameObject.gameObject.SetActive(value);
    }

    public Padding maskPadding
    {
        get => _maskPadding;
        set
        {
            var sizeDelta = defaultMaskSizeDelta;
            var position = defaultMaskPosition;

            sizeDelta.x -= value.left + value.right;
            sizeDelta.y -= value.top + value.bottom;
            
            if (value.left != 0)
                position.x += value.left;
            
            if (value.bottom != 0)
                position.y += value.bottom;
            
            maskRectTransform.sizeDelta = sizeDelta;
            maskRectTransform.localPosition = position;
            
            _maskPadding = value;
        }
    }
    
    private GameObject pageDimmerGameObject;
    private REPOScrollView scrollView;

    private Vector2 defaultMaskSizeDelta, defaultMaskPosition;
    private Padding _maskPadding;
    
    public void OpenPage(bool openOnTop) => MenuAPI.OpenPage(menuPage, openOnTop);
    
    public void ClosePage(bool closePagesAddedOnTop)
    {
        if (closePagesAddedOnTop)
            MenuAPI.CloseAllPagesAddedOnTop();
        
        menuPage.PageStateSet(MenuPage.PageState.Closing);
        
        if (REPOReflection.menuPage_PageUnderThisPage.GetValue(menuPage) is MenuPage parentPage)
            MenuManager.instance.PageSetCurrent(parentPage.menuPageIndex, parentPage);
    }
    
    public void AddElement(MenuAPI.BuilderDelegate builderDelegate) => builderDelegate?.Invoke(transform);
    
    public void AddElement(RectTransform elementRectTransform, Vector2 localPosition = default)
    {
        elementRectTransform.SetParent(transform);
        elementRectTransform.localPosition = localPosition;
    }
    
    public void AddElementToScrollView(ScrollViewBuilderDelegate scrollViewBuilderDelegate)
    {
        if (scrollViewBuilderDelegate?.Invoke(menuScrollBox.scroller)?.gameObject.AddComponent<REPOScrollViewElement>() is { } repoScrollViewElement)
            repoScrollViewElement.onVisibilityChanged = scrollView.UpdateElements;
    }
    
    public void AddElementToScrollView(RectTransform elementRectTransform, Vector2 localPosition = default)
    {
        elementRectTransform.SetParent(menuScrollBox.scroller);
        elementRectTransform.localPosition = localPosition;

        if (elementRectTransform.gameObject.AddComponent<REPOScrollViewElement>() is { } repoScrollViewElement)
            repoScrollViewElement.onVisibilityChanged = scrollView.UpdateElements;
    }
    
    private void Awake()
    {
        menuPage = GetComponent<MenuPage>();
        headerTMP = GetComponentInChildren<TextMeshProUGUI>();
        menuScrollBox = GetComponentInChildren<MenuScrollBox>();
        
        rectTransform = (RectTransform) new GameObject("Page Content", typeof(RectTransform)).transform;
        rectTransform.SetParent(transform);
        
        transform.Find("Panel").SetParent(rectTransform);
        headerTMP.transform.parent.SetParent(rectTransform);
        menuScrollBox.transform.SetParent(rectTransform);
        
        pageDimmerGameObject = Instantiate(REPOTemplates.pageDimmerTemplate, transform).gameObject;
        pageDimmerGameObject.transform.SetAsFirstSibling();

        menuPage.menuPageIndex = (MenuPageIndex) (-1);

        var scroller = menuScrollBox.scroller;
        
        for (var i = 2; i < scroller.childCount; i++)
            Destroy(scroller.GetChild(i).gameObject);

        scrollView = scroller.gameObject.AddComponent<REPOScrollView>();
        scrollView.popupPage = this;
        
        maskRectTransform = (RectTransform) scroller.parent;

        defaultMaskSizeDelta = maskRectTransform.sizeDelta;
        defaultMaskPosition = maskRectTransform.localPosition;

        maskPadding = new Padding(0, 0, 0, 25);

        menuScrollBox.scroller.sizeDelta = maskRectTransform.sizeDelta;
        
        scrollBarRectTransform = (RectTransform) menuScrollBox.scrollBar.transform;
        var scrollBarFillRectTransform = (RectTransform) scrollBarRectTransform.Find("Scroll Bar Bg (2)");
        var scrollBarOutlineRectTransform = (RectTransform) scrollBarRectTransform.Find("Scroll Bar Bg (1)");
        
        var mainScrollBarPosition = scrollBarRectTransform.localPosition;
        mainScrollBarPosition.y = maskRectTransform.localPosition.y;
        scrollBarRectTransform.localPosition = mainScrollBarPosition;

        var scrollBarSize = scrollBarRectTransform.sizeDelta;
        scrollBarSize.y = maskRectTransform.sizeDelta.y;
        menuScrollBox.scrollBarBackground.sizeDelta = scrollBarFillRectTransform.sizeDelta = scrollBarRectTransform.sizeDelta = scrollBarSize;

        scrollBarOutlineRectTransform.sizeDelta = scrollBarSize + new Vector2(4f, 4f);
        
        Destroy(GetComponent<MenuPageSettingsPage>());
        gameObject.AddComponent<MenuPageSettings>();
    }

    private void Start()
    {
        REPOReflection.menuScrollBox_scrollerEndPosition.SetValue(menuScrollBox, 0);
        menuScrollBox.scroller.localPosition = menuScrollBox.scroller.localPosition with { y = 0 };
        
        REPOReflection.menuPage_ScrollBoxes.SetValue(menuPage, 2);
    }
}