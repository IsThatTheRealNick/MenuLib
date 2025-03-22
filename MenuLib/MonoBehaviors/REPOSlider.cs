using MenuLib.Interfaces;
using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOSlider : MonoBehaviour, IREPOElement
{
    public RectTransform rectTransform { get; private set; }

    public TextMeshProUGUI labelTMP;

    private RectTransform barRectTransform, barPointerRectTransform;
    private TextMeshProUGUI valueTMP, maskedValueTMP;
    
    private void Awake()
    {
        rectTransform = transform as RectTransform;
        labelTMP = GetComponentInChildren<TextMeshProUGUI>();
        valueTMP = transform.Find("Bar Text").GetComponent<TextMeshProUGUI>();
        maskedValueTMP = transform.Find("MaskedText").GetComponentInChildren<TextMeshProUGUI>();
        barPointerRectTransform = (RectTransform) transform.Find("Bar Pointer").transform;

        var horizontalShift = Vector3.right * 5.3f;
        
        labelTMP.rectTransform.localPosition -= horizontalShift;
        transform.Find("SliderBG").localPosition -= horizontalShift;

        maskedValueTMP.rectTransform.parent.localPosition = valueTMP.rectTransform.localPosition -= horizontalShift;
        
        var bar = transform.Find("Bar");
        bar.localPosition -= horizontalShift;
        
        barRectTransform = (RectTransform) bar.Find("RawImage");
        barRectTransform.pivot = Vector2.zero;
        barRectTransform.localPosition = new Vector2(0f, -5f);

        var labelSizeDelta = labelTMP.rectTransform.sizeDelta;
        labelSizeDelta.y -= 10;
        labelTMP.rectTransform.sizeDelta = labelSizeDelta;
        
        Destroy(bar.Find("Extra Bar").gameObject);
        Destroy(GetComponent<MenuSliderMicrophone>());
        Destroy(GetComponent<MenuSlider>());
    }
}