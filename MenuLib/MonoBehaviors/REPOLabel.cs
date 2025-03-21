using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOLabel : MonoBehaviour
{
    public RectTransform rectTransform;
    public TextMeshProUGUI labelTMP;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        labelTMP = GetComponentInChildren<TextMeshProUGUI>();

        labelTMP.rectTransform.pivot = rectTransform.pivot = Vector2.zero;
        labelTMP.rectTransform.sizeDelta = rectTransform.sizeDelta = new Vector2(200f, 30f);
        
        labelTMP.fontSize = 30;
        labelTMP.enableWordWrapping = labelTMP.enableAutoSizing = false;
        labelTMP.alignment = TextAlignmentOptions.Left;
        labelTMP.margin = Vector4.zero;
    }

    private void Start() => labelTMP.rectTransform.localPosition = Vector2.zero;
}