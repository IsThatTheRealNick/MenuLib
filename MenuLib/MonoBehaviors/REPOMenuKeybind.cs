using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOMenuKeybind : MonoBehaviour
{
    internal Action<Key> onValueChanged;

    internal TextMeshProUGUI headerTMP;
    
    private MenuPage menuPage;
    private MenuBigButton menuBigButton;
    private TextMeshProUGUI buttonTMP;
    private Button button;

    private Key currentKey;

    private InputActionRebindingExtensions.RebindingOperation currentRebindingOperation;
    private float timeSinceRebind;
    
    private static Key ParseKeyName(string keyName)
    {
        Debug.Log(keyName);
        
        var key = keyName switch
        {
            "-" => Key.Minus,
            "+" => Key.NumpadPlus,
            "," => Key.Comma,
            "." => Key.Period,
            "=" => Key.Equals,
            "`" => Key.Backquote,
            "0" => Key.Digit0,
            "1" => Key.Digit1,
            "2" => Key.Digit2,
            "3" => Key.Digit3,
            "4" => Key.Digit4,
            "5" => Key.Digit5,
            "6" => Key.Digit6,
            "7" => Key.Digit7,
            "8" => Key.Digit8,
            "9" => Key.Digit9,
            "/" => Key.Slash,
            "\\" => Key.Backslash,
            "Right Alt" => Key.RightAlt,
            "Left Control" => Key.LeftCtrl,
            "Right Control" => Key.RightCtrl,
            "Left System" => Key.LeftWindows,
            "Right System" => Key.RightWindows,
            _ => Key.None
        };

        if (key != Key.None)
            return key;

        return Enum.TryParse(keyName.Replace(" ", string.Empty), out key) ? key : Key.None;
    }
    
    internal void Initialize(Key startingValue)
    {
        menuPage = GetComponentInParent<MenuPage>();
        menuBigButton = GetComponent<MenuBigButton>();
        headerTMP = GetComponentInChildren<TextMeshProUGUI>();
        buttonTMP = AccessTools.Field(typeof(MenuButton), "buttonText").GetValue(menuBigButton.menuButton) as TextMeshProUGUI;
        button = GetComponentInChildren<Button>();

        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(OnClick);

        currentKey = startingValue;
        UpdateKeybindLabel();
    }
    
    internal void SetHeader(string label) => headerTMP.text = label;

    internal void UpdateKeybindLabel()
    {
        var sanitizedName = currentKey.ToString();

        sanitizedName = sanitizedName.Replace("Digit", string.Empty).Replace("Numpad", string.Empty).Replace("Slash", "/").Replace("Backslash", "\\");
        sanitizedName = Regex.Replace(sanitizedName, "([a-z])([A-Z])", "$1 $2");
        
        buttonTMP.text = menuBigButton.buttonName = sanitizedName;
    }

    private void Update()
    {
        if (currentRebindingOperation == null)
            return;
        
        timeSinceRebind += Time.deltaTime;

        if (!Mouse.current.leftButton.wasPressedThisFrame || timeSinceRebind <= .1f)
            return;
        
        currentRebindingOperation.Cancel();
        currentRebindingOperation = null;
    }

    private void OnClick()
    {
        currentRebindingOperation?.Cancel();
        
        menuBigButton.state = MenuBigButton.State.Edit;
        
        var templateAction = new InputAction("Template Action", InputActionType.Button, "<Keyboard>/anyKey");
        templateAction.Disable();
        
        currentRebindingOperation = templateAction.PerformInteractiveRebinding()
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(.1f)
            .OnComplete(operation =>
            {
                currentKey = ParseKeyName(InputControlPath.ToHumanReadableString(templateAction.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice));
                
                onValueChanged?.Invoke(currentKey);
                
                menuBigButton.state = MenuBigButton.State.Main;
                MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, menuPage, 0.2f);
                UpdateKeybindLabel();
                
                operation.Dispose();
                templateAction.Dispose();
            })
            .OnCancel(operation =>
            {
                menuBigButton.state = MenuBigButton.State.Main;
                MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, menuPage, 0.2f);
                UpdateKeybindLabel();
                
                operation.Dispose();
                templateAction.Disable();
            }).Start();
        timeSinceRebind = 0;
    }
}