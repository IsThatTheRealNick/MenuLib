using MenuLib.Interfaces;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOSpacer : MonoBehaviour, IREPOElement
{
    public RectTransform rectTransform { get; private set; }
    
    private void Awake() => rectTransform = transform as RectTransform;
}