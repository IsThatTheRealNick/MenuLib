using System;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOMenuToggle : MonoBehaviour
{
    internal Action<bool> onValueChanged;

    private TextMeshProUGUI labelTMP, leftButtonTMP, rightButtonTMP;
    private RectTransform optionBox, optionBoxBehind;

    private Vector3 targetPosition, targetScale;

    private readonly Vector3 leftPosition = new(37.8f, 12.3f),
        rightPosition = new(112.644f, 12.3f),
        leftScale = new(73f, 22f, 1f),
        rightScale = new(74f, 22f, 1f);
    
    internal void Initialize(bool startingValue)
    {
        labelTMP = GetComponentInChildren<TextMeshProUGUI>();
        optionBox = (RectTransform) transform.Find("Option Box");
        optionBoxBehind = (RectTransform) transform.Find("Option Box Behind");
        
        labelTMP.rectTransform.sizeDelta = labelTMP.rectTransform.sizeDelta with { y = labelTMP.rectTransform.sizeDelta.y - 4};

        var buttons = GetComponentsInChildren<Button>();
        
        var leftButton = buttons[0];
        leftButtonTMP = leftButton.GetComponentInChildren<TextMeshProUGUI>();
        leftButton.onClick = new Button.ButtonClickedEvent();
        leftButton.onClick.AddListener(() => SetState(true, true));
        
        var rightButton = buttons[1];
        rightButtonTMP = rightButton.GetComponentInChildren<TextMeshProUGUI>();
        rightButton.onClick.AddListener(() => SetState(false, true));
        
        SetState(startingValue, false);
    }

    internal void SetState(bool state, bool invokeCallback)
    {
        targetPosition = state ? leftPosition : rightPosition;
        targetScale = state ? leftScale : rightScale;
        
        if (invokeCallback)
            onValueChanged.Invoke(state);
    }

    internal void SetLeftButtonText(string text) => leftButtonTMP.text = text;
    
    internal void SetRightButtonText(string text) => rightButtonTMP.text = text;
    
    internal void SetLabel(string label) => labelTMP.text = label;

    private void Update()
    {
        if (!optionBox || !optionBoxBehind)
            return;
        
        optionBox.localPosition = Vector3.Lerp(optionBox.localPosition, targetPosition, 20f * Time.deltaTime);
        optionBox.localScale = Vector3.Lerp(optionBox.localScale, targetScale / 10f, 20f * Time.deltaTime);
        optionBoxBehind.localPosition = Vector3.Lerp(optionBoxBehind.localPosition, targetPosition, 20f * Time.deltaTime);
        optionBoxBehind.localScale = Vector3.Lerp(optionBoxBehind.localScale, new Vector3(targetScale.x + 4f, targetScale.y + 2f, 1f) / 10f, 20f * Time.deltaTime);
    }
}