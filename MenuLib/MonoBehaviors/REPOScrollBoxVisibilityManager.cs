using UnityEngine;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOScrollBoxVisibilityManager : MonoBehaviour
{
    internal RectTransform scroller;
    internal RectTransform mask;
    
    private void Update()
    {
        if (!mask || !scroller || scroller.childCount == 0)
            return;

        for (var i = 0; i < scroller.childCount; i++)
        {
            if (i <= 2)
                continue;
            
            var element = scroller.GetChild(i);
            
            var currentPosition = element.position;
            element.gameObject.SetActive(currentPosition.y > mask.position.y - 100f && currentPosition.y < mask.position.y + mask.sizeDelta.y + 100f);
        }
    }
}