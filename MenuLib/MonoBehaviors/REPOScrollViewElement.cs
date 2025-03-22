using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOScrollViewElement : MonoBehaviour
{
    public RectTransform rectTransform;

    public Action onSettingChanged;

    public float? topPadding
    {
        get => _topPadding;
        set
        {
            if (_topPadding.HasValue && value.HasValue && Math.Abs(_topPadding.Value - value.Value) < float.Epsilon)
                return;
            
            onSettingChanged?.Invoke();
            
            _topPadding = value;
        }
    }
    public float? bottomPadding
    {
        get => _bottomPadding;
        set
        {
            if (_bottomPadding.HasValue && value.HasValue && Math.Abs(_bottomPadding.Value - value.Value) < float.Epsilon)
                return;
            
            onSettingChanged?.Invoke();
            
            _bottomPadding = value;
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
    private float? _topPadding, _bottomPadding;
    
    private void Awake() => rectTransform = transform as RectTransform;
}