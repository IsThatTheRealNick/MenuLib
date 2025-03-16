using System;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using MenuLib.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOMenuSliderFloat : MonoBehaviour
{
    private static readonly FieldInfo getMenuID = AccessTools.Field(typeof(MenuSelectableElement), "menuID");
    
    internal MenuPage menuPage;
    internal REPOTextScroller textScroller;
    internal TextMeshProUGUI descriptionTextTMP;

    internal REPOBarState barState;
    
    internal Action<float> onValueChanged;
    internal Action<int> onOptionChanged;
    internal string[] options;

    internal string displayPrefix, displayPostfix;

    internal float min, max, precision;
    internal int decimalPlaces;
    
    private TextMeshProUGUI headerTMP, barTMP, maskedTMP;
    private MenuSelectableElement menuSelectableElement;
    private RectTransform rectTransform, maskedRectTransform, barRectTransform, barSizeRectTransform;
    private RectTransform bigTextOutline, bigTextFill;
    
    private Transform barPointer;
    
    private Vector2 barSizeDelta = new(0f, 10f);
    
    private float currentValue, previousValue;

    private bool isHovering;

    private bool hasValueChanged => Mathf.Abs(currentValue - previousValue) >= Mathf.Epsilon;
    
    internal void Initialize(float defaultValue)
    {
        rectTransform = (RectTransform) transform;
        headerTMP = GetComponentInChildren<TextMeshProUGUI>();
        
        headerTMP.rectTransform.sizeDelta = new Vector2(200, 40);
        
        barTMP = transform.Find("Bar Text").GetComponent<TextMeshProUGUI>();
        maskedRectTransform = (RectTransform) transform.Find("MaskedText");
        maskedTMP = maskedRectTransform.GetComponentInChildren<TextMeshProUGUI>();
        
        descriptionTextTMP = transform.Find("Big Setting Text").GetComponent<TextMeshProUGUI>();
        
        descriptionTextTMP.alignment = TextAlignmentOptions.Left;
        descriptionTextTMP.enableAutoSizing = descriptionTextTMP.enableWordWrapping = false;
        descriptionTextTMP.overflowMode = TextOverflowModes.Masking;
        descriptionTextTMP.fontSize -= 5;

        textScroller = descriptionTextTMP.gameObject.AddComponent<REPOTextScroller>();
        textScroller.textMeshPro = descriptionTextTMP;
        
        var descriptionSizeDelta = descriptionTextTMP.rectTransform.sizeDelta; 
        descriptionTextTMP.rectTransform.sizeDelta = descriptionSizeDelta with { y = descriptionSizeDelta.y - 4f };
        
        barSizeRectTransform = (RectTransform) transform.Find("BarSize");

        var background = transform.Find("SliderBG");
        bigTextOutline = (RectTransform) background.Find("RawImage (3)");
        bigTextFill = (RectTransform) background.Find("RawImage (2)");
        
        Destroy(background.Find("RawImage (4)").gameObject);
        Destroy(background.Find("RawImage (5)").gameObject);
        
        menuSelectableElement = GetComponent<MenuSelectableElement>();
        
        barPointer = transform.Find("Bar Pointer");

        var bar = transform.Find("Bar");
        
        barRectTransform = (RectTransform) bar.GetChild(0).transform;
        barRectTransform.pivot = new Vector2(0, .5f);
        barRectTransform.localPosition = Vector3.zero;
        
        Destroy(bar.Find("Extra Bar").gameObject);

        var buttons = GetComponentsInChildren<Button>();
        var decrementButton = buttons[0];
        decrementButton.onClick.AddListener(Decrement);
        
        var incrementButton = buttons[1];
        incrementButton.onClick.AddListener(Increment);
        
        previousValue = currentValue = defaultValue;
        
        var precisionAsString = precision.ToString(CultureInfo.InvariantCulture);
        var decimalIndex = precisionAsString.IndexOf('.');

        if (decimalIndex == -1)
            decimalPlaces = 0;
        else
            decimalPlaces = precisionAsString.Length - decimalIndex - 1;

        UpdateBarPosition();
        UpdateBarLabel();
    }
    
    internal void SetHeader(string text) => headerTMP.text = text;

    internal void SetDescriptionText(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            bigTextOutline.sizeDelta = new Vector2(108f, 29.5f);
            bigTextFill.sizeDelta = new Vector2(109.8f, 32f);
        }
        else
        {
            bigTextOutline.sizeDelta = new Vector2(108f, 15.5f);
            bigTextFill.sizeDelta = new Vector2(109.8f, 15.5f);
        }
        
        descriptionTextTMP.text = text;
    }

    internal void UpdateBarState(REPOBarState newBarState)
    {
        barState = newBarState;
        UpdateBarPosition();
    }

    private void Update()
    {
        if (SemiFunc.UIMouseHover(menuPage, barSizeRectTransform, getMenuID.GetValue(menuSelectableElement) as string, 5f, 5f))
        {
            if (!isHovering)
            {
                MenuManager.instance.MenuEffectHover(SemiFunc.MenuGetPitchFromYPos(rectTransform));
                isHovering = true;
            }
                
            SemiFunc.MenuSelectionBoxTargetSet(menuPage, barSizeRectTransform, new Vector2(-3f, 0f), new Vector2(20f, 10f));
            
            OnHover();
        }
        else
        {
            isHovering = false;
            
            if (barPointer.gameObject.activeSelf)
            {
                var currentPosition = barPointer.localPosition;
                currentPosition.x = -1000f;
                barPointer.localPosition = currentPosition;
                
                barPointer.gameObject.SetActive(false);
            }
        }
        
        if (!hasValueChanged)
            return;
        
        UpdateBarPosition();
        UpdateBarLabel();
        
        previousValue = currentValue;
        
        if (options != null)
            onOptionChanged?.Invoke(Convert.ToInt32(currentValue));
        else
            onValueChanged?.Invoke(currentValue);
    }

    private void OnHover()
    {
        if (!barPointer.gameObject.activeSelf)
            barPointer.gameObject.SetActive(true);

        var pointInRect = SemiFunc.UIMouseGetLocalPositionWithinRectTransform(barSizeRectTransform).x;
        
        var multiplier = max - min;
        var steps = precision / multiplier;
        var normalized = Mathf.Round(Mathf.Clamp01(pointInRect / barSizeRectTransform.sizeDelta.x) / steps) * steps;
        
        var newXPos = Mathf.Clamp(barSizeRectTransform.localPosition.x + normalized * barSizeRectTransform.sizeDelta.x, barSizeRectTransform.localPosition.x,
            barSizeRectTransform.localPosition.x + barSizeRectTransform.sizeDelta.x) - 2f;

        barPointer.localPosition = barPointer.localPosition with { x = newXPos };

        if (!Input.GetMouseButton(0))
            return;
        
        currentValue = min + normalized * multiplier;
        
        if (hasValueChanged)
            MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, menuPage);
    }

    private void UpdateBarPosition()
    {
        switch (barState)
        {
            case REPOBarState.UpdateWithValue:
                SetBarPosition((currentValue - min) / (max - min));
                break;
            case REPOBarState.StaticAtMinimum:
                SetBarPosition(0);
                break;
            case REPOBarState.StaticAtMaximum:
                SetBarPosition(1);
                break;
        }
    }
    
    private void SetBarPosition(float normalizedValue)
    {
        barSizeDelta.x = normalizedValue * 100f;
        maskedRectTransform.sizeDelta = barRectTransform.sizeDelta = barSizeDelta;
    }

    private void Increment()
    {
        var increment = options != null ? 1 : precision;
        
        var newValue = currentValue + increment;
        
        if (Math.Abs(max - currentValue) < float.Epsilon)
            newValue = min;
        else if (newValue > max)
            newValue = max;

        currentValue = newValue;
    }
    
    private void Decrement()
    {
        var increment = options != null ? 1 : precision;
        
        var newValue = currentValue - increment;
            
        if (Math.Abs(currentValue - min) < float.Epsilon)
            newValue = max;
        else if (newValue < min)
            newValue = min;

        currentValue = newValue;
    }

    private void UpdateBarLabel()
    {
        if (options != null)
        {
            barTMP.text = maskedTMP.text = (displayPrefix ??= string.Empty) + options[Convert.ToInt32(currentValue)] + (displayPostfix ??= string.Empty);
            return;
        }
        
        var valueAsString = currentValue.ToString(CultureInfo.InvariantCulture);
        
        valueAsString = int.TryParse(valueAsString, out var value) ? value.ToString() : currentValue.ToString($"F{decimalPlaces}", CultureInfo.InvariantCulture);
        
        barTMP.text = maskedTMP.text = (displayPrefix ??= string.Empty) + valueAsString + (displayPostfix ??= string.Empty);
    }
}