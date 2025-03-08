using System;
using System.Collections;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MenuLib;

public sealed class REPOToggle : REPOElement
{
    public string labelText { get; private set; }
    public string onText { get; private set; }
    public string offText { get; private set; }
    
    public Action<bool> onClick { get; private set; }
    
    private MenuTwoOptions menuTwoOptions;
    private TextMeshProUGUI toggleTextTMP, onTextTMP, offTextTMP;
    
    private readonly bool defaultValue;
    
    public REPOToggle(string labelText, Action<bool> onClick, string onText, string offText, bool defaultValue)
    {
        this.labelText = labelText;
        this.onClick = onClick;
        this.onText = onText;
        this.offText = offText;
        this.defaultValue = defaultValue;
    }
    
    public void SetLabelText(string newLabelText)
    {
        if (toggleTextTMP)
            toggleTextTMP.text = newLabelText;
        
        labelText = newLabelText;
    }
    
    public void SetOnText(string newOnText)
    {
        if (onTextTMP)
            onTextTMP.text = newOnText;
        
        onText = newOnText;
    }
    
    public void SetOffText(string newOffText)
    {
        if (offTextTMP)
            offTextTMP.text = newOffText;
        
        offText = newOffText;
    }

    public void SetOnClick(Action<bool> newOnClick)
    {
        if (menuTwoOptions)
        {
            menuTwoOptions.onOption1 = new Button.ButtonClickedEvent();
            menuTwoOptions.onOption1.AddListener(() => newOnClick?.Invoke(true));
            
            menuTwoOptions.onOption2 = new Button.ButtonClickedEvent();
            menuTwoOptions.onOption2.AddListener(() => newOnClick?.Invoke(false));
        }
        
        onClick = newOnClick;
    }
    
    public override RectTransform GetReference() => MenuAPI.toggleTemplate;
    
    public override void SetDefaults()
    {
        menuTwoOptions = transform.GetComponent<MenuTwoOptions>();
        toggleTextTMP = transform.GetComponentInChildren<TextMeshProUGUI>();
        onTextTMP = menuTwoOptions.option1TextMesh;
        offTextTMP = menuTwoOptions.option2TextMesh;
        
        transform.name = $"Toggle Button - {labelText}";
        
        menuTwoOptions.customEvents = true;
        menuTwoOptions.settingSet = false;

        toggleTextTMP.rectTransform.sizeDelta = toggleTextTMP.rectTransform.sizeDelta with { y = toggleTextTMP.rectTransform.sizeDelta.y - 2};
        
        SetLabelText(labelText);
        SetOnText(onText);
        SetOffText(offText);

        Object.Destroy(transform.GetComponent<AudioButtonPushToTalk>());
        
        menuTwoOptions.StartCoroutine(SetupLate());

        afterBeingParented = menuPage =>
        {
            var menuButtons = transform.GetComponentsInChildren<MenuButton>();

            foreach (var menuButton in menuButtons)
                AccessTools.Field(typeof(MenuButton), "parentPage").SetValue(menuButton, menuPage);
        };
        
        return;
        
        IEnumerator SetupLate()
        {
            yield return null;
            
            if (defaultValue)
                menuTwoOptions.OnOption1();
            else
                menuTwoOptions.OnOption2();
            
            SetOnClick(onClick);
        }
    }
}