using UnityEngine;

namespace MenuLib.MonoBehaviors;

public class REPOElement : MonoBehaviour
{
    public RectTransform rectTransform { get; protected set; }
    public REPOScrollViewElement repoScrollViewElement
    {
        get
        {
            if (_repoScrollViewElement)
                return _repoScrollViewElement;
            
            return _repoScrollViewElement = GetComponent<REPOScrollViewElement>();
        }
    }
    
    private REPOScrollViewElement _repoScrollViewElement;
}