using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOScrollViewElement : MonoBehaviour
{
    public RectTransform rectTransform;

    public Action onSettingChanged;

    public float? overrideHeight
    {
        get => _overrideHeight;
        set
        {
            if (_overrideHeight.HasValue && value.HasValue && Math.Abs(_overrideHeight.Value - value.Value) < float.Epsilon)
                return;
            
            onSettingChanged?.Invoke();
            
            _overrideHeight = value;
        }
    }
    
    public bool visibility
    {
        get => _visibility;
        set
        {
            if (_visibility == value)
                return;
            
            onSettingChanged?.Invoke();
            gameObject.SetActive(value);
            _visibility = value;
        }
    }

    private bool _visibility = true;
    private float? _overrideHeight;
    
    private void Awake() => rectTransform = transform as RectTransform;
}