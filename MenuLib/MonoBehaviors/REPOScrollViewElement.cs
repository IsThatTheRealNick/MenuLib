using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOScrollViewElement : MonoBehaviour
{
    public RectTransform rectTransform;

    public Action onVisibilityChanged;

    public bool visibility
    {
        get => _visibility;
        set
        {
            if (_visibility == value)
                return;
            
            onVisibilityChanged?.Invoke();
            gameObject.SetActive(value);
            _visibility = value;
        }
    }

    private bool _visibility = true;
    
    private void Awake() => rectTransform = transform as RectTransform;
}