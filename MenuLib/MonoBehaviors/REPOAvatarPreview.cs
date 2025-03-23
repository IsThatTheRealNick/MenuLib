using UnityEngine;

namespace MenuLib.MonoBehaviors;

public class REPOAvatarPreview : REPOElement
{
    private void Awake()
    {
        rectTransform = transform as RectTransform;
    }
}