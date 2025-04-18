using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MenuLib.MonoBehaviors;

public sealed class REPOInputStringSystem : MonoBehaviour
{
    public struct Colors
    {
        public Color focusedColor = Color.white,
            hoverColor = Color.white,
            unfocusedColor = new(0.5f, 0.5f, 0.5f),
            placeholderColor = new(1f, 0.6f, 0.4f),
            characterAddedColor = new(0.5f, 1f, 0.5f),
            characterRemovedColor = new (1f, 0.5f, 0.5f);

        public Colors() { }
    }
    
    public TMP_Text inputTMP;
    public RectTransform maskRectTransform;
    
    public Action<string> onValueChanged;

    public Colors colors = new();
    
    public string currentValue { get; private set; }
    public string placeholder = string.Empty;
    
    public bool notifyOnSubmit;
    public bool isHovering { get; private set; }

    public bool isFocused
    {
        get => _isFocused;
        set
        {
            switch (value)
            {
                case false when _isFocused:
                {
                    MoveTMP(true);
                    break;
                }
                case true when !isFocused:
                {
                    if (IsTMPInMask())
                        MoveTMP(true);
                    else
                        MoveTMP(true, -inputTMP.GetPreferredValues($"{currentValue}<b>|</b>").x + maskRectTransform.rect.width - 6);
                    
                    break;
                }
            }

            _isFocused = value;
        }
    }
    
    private string previousValue;

    private float timeSinceCharacterAdded = 1, timeSinceCharacterRemoved = 1;
    
    private bool shouldUsePlaceholder => string.IsNullOrEmpty(currentValue);
    
    private bool pressedSubmit;
    private bool _isFocused;
    
    public void SetValue(string value, bool notify)
    {
        previousValue = currentValue = value;

        if (notify)
            onValueChanged?.Invoke(value);
    }

    public void SetHovering(bool value)
    {
        if (value && !isHovering)
            MenuManager.instance.MenuEffectHover(SemiFunc.MenuGetPitchFromYPos(inputTMP.rectTransform));

        isHovering = value;
    }
    
    private void Update()
    {
        if (!inputTMP)
            return;
        
        HandleInput();
        UpdateColors();

        if (pressedSubmit)
        {
            isFocused = false;
            pressedSubmit = false;
            
            if (notifyOnSubmit)
                onValueChanged?.Invoke(currentValue);
        }
        
        if (!notifyOnSubmit && previousValue != currentValue)
            onValueChanged?.Invoke(currentValue);
        
        if (isFocused)
        {
            inputTMP.text = currentValue;
            
            if (timeSinceCharacterAdded < .5f || timeSinceCharacterRemoved < .5f || Mathf.Sin(Time.time * 10f) > 0)
                inputTMP.text += "<b>|</b>";
        }
        else
            inputTMP.text = shouldUsePlaceholder ? placeholder : currentValue;
            
        
        previousValue = currentValue;
    }

    private void HandleInput()
    {
        if (timeSinceCharacterAdded < 1)
            timeSinceCharacterAdded += Time.deltaTime;
        
        if (timeSinceCharacterRemoved < 1)
            timeSinceCharacterRemoved += Time.deltaTime;
        
        if (!isFocused || Keyboard.current.ctrlKey.isPressed)
            return;
        
        var character = Input.inputString;
        var previousString = currentValue;
        
        switch (character)
        {
            case "\b":
            {
                if (currentValue.Length <= 0)
                    return;
                
                currentValue = currentValue.Remove(currentValue.Length - 1, 1);
                timeSinceCharacterRemoved = 0;
                
                if (IsTMPInMask())
                    MoveTMP(true);
                else
                    MoveTMP(false, -CalculateTMPDifference(previousString, currentValue));

                return;
            }
            case "\r":
            {
                pressedSubmit = true;
                return;
            }
        }

        if (string.IsNullOrEmpty(character))
            return;
        
        currentValue += character;
        timeSinceCharacterAdded = 0;
        
        if (IsTMPInMask())
            MoveTMP(true);
        else
            MoveTMP(false, -CalculateTMPDifference(previousString, currentValue));
    }
    
    private void UpdateColors()
    {
        if (timeSinceCharacterAdded < .1f)
        {
            inputTMP.color = colors.characterAddedColor;
            return;
        }
        
        if (timeSinceCharacterRemoved < .1f)
        {
            inputTMP.color = colors.characterRemovedColor;
            return;
        }
        
        if (isHovering)
        {
            inputTMP.color = colors.hoverColor;
            return;
        }

        if (isFocused)
        {
            inputTMP.color = colors.focusedColor;
            return;
        }
        
        inputTMP.color = shouldUsePlaceholder ? colors.placeholderColor : colors.unfocusedColor;
    }
    
    private void MoveTMP(bool reset, float moveBy = 0)
    {
        if (reset)
            inputTMP.transform.localPosition = Vector3.zero;
        
        if (moveBy != 0)
            inputTMP.transform.localPosition += Vector3.right * moveBy;
    }

    private float CalculateTMPDifference(string previousText, string newText) => inputTMP.GetPreferredValues(newText).x - inputTMP.GetPreferredValues(previousText).x;

    private bool IsTMPInMask() => inputTMP.GetPreferredValues(currentValue).x < maskRectTransform.rect.width - 6;
}