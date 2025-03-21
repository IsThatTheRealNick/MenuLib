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

        rectTransform.pivot = Vector2.zero;
        labelTMP.rectTransform.pivot = new Vector2(0f, 0.5f);
        labelTMP.rectTransform.sizeDelta = rectTransform.sizeDelta = new Vector2(200f, 40f);

        labelTMP.enableWordWrapping = labelTMP.enableAutoSizing = false;
        labelTMP.alignment = TextAlignmentOptions.Left;
        labelTMP.margin = Vector4.zero;
    }

    private void Start() => labelTMP.rectTransform.localPosition = Vector2.zero;
}