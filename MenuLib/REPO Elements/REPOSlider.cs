using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MenuLib;

public sealed class REPOSlider : REPOElement
{
    public string text { get; private set; }
    public string description { get; private set; }
    public float min { get; private set; }
    public float max { get; private set; }
    public int precision { get; private set; }

    public string[] options { get; private set; }
    
    public Action<float> onValueChanged { get; private set; }
    public Action<int> onOptionChanged { get; private set; }
    
    internal REPOMenuSliderFloat menuSliderFloat;
    
    private readonly float defaultValue;
    
    public REPOSlider(string text, string description, Action<float> onValueChanged, float min, float max, int precision, float defaultValue)
    {
        this.text = text;
        this.description = description;
        this.onValueChanged = onValueChanged;
        this.min = min;
        this.max = max;
        this.precision = precision;
        this.defaultValue = defaultValue;
    }
    
    public REPOSlider(string text, string description, Action<int> onOptionChanged, string defaultOption, params string[] options)
    {
        this.text = text;
        this.description = description;
        this.onOptionChanged = onOptionChanged;

        var defaultValueIndex = Array.IndexOf(options, defaultOption);

        if (defaultValueIndex == -1)
            defaultValueIndex = 0;
        
        defaultValue = defaultValueIndex;
        this.options = options;
        max = options.Length - 1f;
    }
    
    public REPOSlider SetText(string newText)
    {
        if (menuSliderFloat)
            menuSliderFloat.SetHeader(newText);
        
        text = newText;
        return this;
    }
    
    public REPOSlider SetDescription(string newDescription)
    {
        if (menuSliderFloat)
            menuSliderFloat.SetDescriptionText(newDescription);
        
        description = newDescription;
        return this;
    }
    
    public REPOSlider SetMin(float newMin)
    {
        if (menuSliderFloat)
            menuSliderFloat.min = newMin;
        
        min = newMin;
        return this;
    }
    
    public REPOSlider SetMax(float newMax)
    {
        if (menuSliderFloat)
            menuSliderFloat.max = newMax;
        
        max = newMax;
        return this;
    }
    
    public REPOSlider SetPrecision(int newPrecision)
    {
        if (menuSliderFloat)
        {
            menuSliderFloat.decimalPlaces = newPrecision;
            menuSliderFloat.precision = (float) Math.Pow(10, -newPrecision);
        }
        
        precision = newPrecision;
        return this;
    }
    
    public REPOSlider SetOnValueChanged(Action<float> newOnValueChanged)
    {
        if (menuSliderFloat)
            menuSliderFloat.onValueChanged = newOnValueChanged;
        
        onValueChanged = newOnValueChanged;
        return this;
    }
    
    public REPOSlider SetOnOptionChanged(Action<int> newOnOptionChanged)
    {
        if (menuSliderFloat)
            menuSliderFloat.onOptionChanged = newOnOptionChanged;
        
        onOptionChanged = newOnOptionChanged;
        return this;
    }
    
    public REPOSlider SetOptions(params string[] newOptions)
    {
        if (menuSliderFloat)
            menuSliderFloat.options = newOptions;
        
        options = newOptions;
        return this;
    }
    
    public override RectTransform GetReference() => MenuAPI.sliderTemplate;
    
    public override void SetDefaults()
    {
        Object.Destroy(transform.GetComponent<MenuSliderMicrophone>());
        Object.Destroy(transform.GetComponent<MenuSlider>());
        
        menuSliderFloat = transform.gameObject.AddComponent<REPOMenuSliderFloat>();
        
        transform.name = $"Slider - {text}";
        
        SetMin(min);
        SetMax(max);
        SetPrecision(precision);
        SetOnValueChanged(onValueChanged);
        SetOnOptionChanged(onOptionChanged);
        SetOptions(options);
        
        menuSliderFloat.Initialize(defaultValue);
        SetText(text);
        SetDescription(description);
        
        afterBeingParented = menuPage =>
        {
            menuSliderFloat.menuPage = menuPage;
            
            foreach (var menuButton in transform.GetComponentsInChildren<MenuButton>())
                AccessTools.Field(typeof(MenuButton), "parentPage").SetValue(menuButton, menuPage);
            
        };
    }
}