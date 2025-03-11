using System;
using HarmonyLib;
using REPOConfig.MonoBehaviors;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace MenuLib;

public sealed class REPOKeybind : REPOElement
{
    public string labelText { get; private set; }

    public Action<Key> onValueChanged { get; private set; }
    
    private REPOMenuKeybind menuKeybind;
    private readonly Key defaultValue;
    
    public REPOKeybind(string labelText, Action<Key> onValueChanged, Key defaultValue)
    {
        this.labelText = labelText;
        this.onValueChanged = onValueChanged;
        this.defaultValue = defaultValue;
    }
    
    public REPOKeybind SetLabelText(string newLabelText)
    {
        if (menuKeybind)
            menuKeybind.SetHeader(newLabelText);
        
        labelText = newLabelText;
        return this;
    }
    
    public REPOKeybind SetOnValueChanged(Action<Key> newOnValueChanged)
    {
        if (menuKeybind)
            menuKeybind.onValueChanged = newOnValueChanged;
        
        onValueChanged = newOnValueChanged;
        return this;
    }
    
    public override RectTransform GetReference() => MenuAPI.keybindTemplate;
    
    public override void SetDefaults()
    {
        transform.name = $"Keybind - {labelText}";
        
        menuKeybind = transform.gameObject.AddComponent<REPOMenuKeybind>();
        transform.sizeDelta = transform.sizeDelta with { y = 20 };
        
        menuKeybind.Initialize(defaultValue);
        SetLabelText(labelText);
        SetOnValueChanged(onValueChanged);

        menuKeybind.headerTMP.alignment = TextAlignmentOptions.Left;
        menuKeybind.headerTMP.rectTransform.sizeDelta = new Vector2(200, 40);
        
        Object.Destroy(transform.GetComponent<MenuKeybind>());
        
        afterBeingParented = menuPage =>
        {
            var menuButtons = transform.GetComponentsInChildren<MenuButton>();

            foreach (var menuButton in menuButtons)
                AccessTools.Field(typeof(MenuButton), "parentPage").SetValue(menuButton, menuPage);
        };
    }
}