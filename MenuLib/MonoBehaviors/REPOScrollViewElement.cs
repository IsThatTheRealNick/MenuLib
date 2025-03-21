using System;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOScrollViewElement : MonoBehaviour
{
    public Action onActiveStateChanged;

    private void OnEnable() => onActiveStateChanged?.Invoke();
    
    private void OnDisable() => onActiveStateChanged?.Invoke();
}