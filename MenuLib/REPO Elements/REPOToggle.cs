using System;
using HarmonyLib;
using MenuLib.MonoBehaviors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MenuLib;

public sealed class REPOToggle : REPOElement
{
    public string labelText { get; private set; }
    public string rightButtonText { get; private set; }
    public string leftButtonText { get; private set; }
    
    public Action<bool> onClick { get; private set; }

    private REPOMenuToggle menuToggle;
    
    private readonly bool defaultValue;
    
    public REPOToggle(string labelText, Action<bool> onClick, string rightButtonText, string leftButtonText, bool defaultValue)
    {
        this.labelText = labelText;
        this.onClick = onClick;
        this.rightButtonText = rightButtonText;
        this.leftButtonText = leftButtonText;
        this.defaultValue = defaultValue;
    }
    
    public REPOToggle SetLabelText(string newLabelText)
    { 
        menuToggle?.SetLabel(newLabelText);
        
        labelText = newLabelText;
        return this;
    }
    
    public REPOToggle SetLeftButtonText(string newLeftButtonText)
    {
        menuToggle?.SetLeftButtonText(newLeftButtonText);
        rightButtonText = newLeftButtonText;
        return this;
    }
    
    public REPOToggle SetRightButtonText(string newRightButtonText)
    {
        menuToggle?.SetRightButtonText(newRightButtonText);
        
        leftButtonText = newRightButtonText;
        return this;
    }

    public REPOToggle SetOnClick(Action<bool> newOnClick)
    {
        if (menuToggle)
            menuToggle.onValueChanged = newOnClick;
        
        onClick = newOnClick;
        return this;
    }
    
    public override RectTransform GetReference() => MenuAPI.toggleTemplate;
    
    public override void SetDefaults()
    {
        Object.Destroy(transform.GetComponent<MenuTwoOptions>());
        Object.Destroy(transform.GetComponent<AudioButtonPushToTalk>());

        menuToggle = transform.gameObject.AddComponent<REPOMenuToggle>();
        
        transform.name = $"Toggle Button - {labelText}";
        
        menuToggle.Initialize(defaultValue);
        
        SetLabelText(labelText);
        SetLeftButtonText(rightButtonText);
        SetRightButtonText(leftButtonText);
        SetOnClick(onClick);
        
        afterBeingParented = menuPage => {
            var menuButtons = transform.GetComponentsInChildren<MenuButton>();

            foreach (var menuButton in menuButtons)
                AccessTools.Field(typeof(MenuButton), "parentPage").SetValue(menuButton, menuPage);
        };
    }
}