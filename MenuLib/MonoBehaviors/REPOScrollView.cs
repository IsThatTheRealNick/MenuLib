using UnityEngine;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOScrollView : MonoBehaviour
{
    public void UpdateElements()
    {
        var yPosition = 0f;
        
        for (var i = 2; i < transform.childCount; i++)
        {
            var child = (RectTransform) transform.GetChild(i);
            
            if (!child.gameObject.activeSelf)
                continue;

            var localPosition = child.localPosition;
            
            localPosition.y = yPosition;
            yPosition -= child.sizeDelta.y;
            
            child.localPosition = localPosition;
            
            //Update scrollbar
        }
    }
    
    private void OnTransformChildrenChanged() => UpdateElements();
}