using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOSpacer : REPOElement
{
    private void Awake()
    {
        rectTransform = (RectTransform) transform;
        rectTransform.pivot = Vector2.zero;
    }
}