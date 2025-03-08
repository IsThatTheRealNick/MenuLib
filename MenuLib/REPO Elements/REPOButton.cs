using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MenuLib;

public sealed class REPOButton : REPOElement
{
    public string text { get; private set; }
    public Action onClick { get; private set; }
    
    internal MenuButton menuButton;

    private MenuButtonPopUp menuButtonPopUp;
    private TextMeshProUGUI buttonTextTMP;
    private Button button;
    
    public REPOButton(string text, Action onClick)
    {
        this.text = text;
        this.onClick = onClick;
    }
    
    public void SetText(string newText)
    {
        if (buttonTextTMP)
        {
            buttonTextTMP.text = newText;
            transform.sizeDelta = buttonTextTMP.GetPreferredValues();
        }
        
        text = newText;
    }

    public void SetOnClick(Action newOnClick)
    {
        if (button && newOnClick != null)
        {
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new UnityAction(newOnClick));
        }
        
        onClick = newOnClick;
    }

    public void OpenDialog(string header, string content, Action onConfirm)
    {
        if (!menuButtonPopUp) 
            menuButtonPopUp = transform.gameObject.AddComponent<MenuButtonPopUp>();   
        
        menuButtonPopUp.option1Event = new UnityEvent();
        menuButtonPopUp.option1Event.AddListener(new UnityAction(onConfirm));

        MenuManager.instance.PagePopUpTwoOptions(menuButtonPopUp, header, menuButtonPopUp.headerColor, content, "Yes", "No");
    }
    
    public override RectTransform GetReference() => MenuAPI.buttonTemplate;
    
    public override void SetDefaults()
    {
        menuButton = transform.GetComponent<MenuButton>();
        buttonTextTMP = AccessTools.Field(typeof(MenuButton), "buttonText").GetValue(menuButton) as TextMeshProUGUI;
        button = transform.GetComponent<Button>();
        
        transform.name = $"Menu Button - {text}";
        
        SetText(text);
        SetOnClick(onClick);
        
        Object.Destroy(transform.GetComponent<MenuButtonPopUp>());

        afterBeingParented = menuPage =>
        {
            AccessTools.Field(typeof(MenuButton), "parentPage").SetValue(menuButton, menuPage);
        };
    }
}