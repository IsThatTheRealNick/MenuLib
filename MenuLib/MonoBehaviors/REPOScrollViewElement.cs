using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOScrollViewElement : MonoBehaviour
{
    public RectTransform rectTransform;
    
    public float topPadding
    {
        get => _topPadding;
        set
        {
            if (Math.Abs(_topPadding - value) < float.Epsilon)
                return;
            
            _topPadding = value;
            onSettingChanged?.Invoke();
        }
    }
    public float bottomPadding
    {
        get => _bottomPadding;
        set
        {
            if (Math.Abs(_bottomPadding - value) < float.Epsilon)
                return;
            
            _bottomPadding = value;
            onSettingChanged?.Invoke();
        }
    }
    
    public bool visibility
    {
        get => _visibility;
        set
        {
            if (_visibility == value)
                return;
            
            _visibility = value;
            gameObject.SetActive(value);
            onSettingChanged?.Invoke();
        }
    }

    internal Action onSettingChanged;
    
    private bool _visibility = true;
    private float _topPadding, _bottomPadding;
    
    private void Awake() => rectTransform = transform as RectTransform;
}