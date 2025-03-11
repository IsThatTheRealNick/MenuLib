using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MenuLib;

public abstract class REPOElement
{
    public Vector2 position { get; private set; }

    public Action<MenuPage> afterBeingParented;
    
    internal RectTransform transform;
    
    public REPOElement SetPosition(Vector2 newLocalPosition)
    {
        if (transform)
            transform.localPosition = newLocalPosition;
        
        position = newLocalPosition;
        return this;
    }
    
    public abstract RectTransform GetReference();

    public virtual void SetDefaults() { }
    
    public RectTransform Instantiate()
    {
        transform = Object.Instantiate(GetReference());
        
        SetDefaults();
        
        return transform;
    }
}