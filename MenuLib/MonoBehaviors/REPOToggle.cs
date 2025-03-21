using System;
using MenuLib.Interfaces;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace MenuLib.MonoBehaviors;

public sealed class REPOToggle : MonoBehaviour, IREPOElement
{
    private static readonly Vector3 leftPosition = new(137.8f, 12.3f), leftScale = new(73f, 22f, 1f), rightPosition = new(212.644f, 12.3f), rightScale = new(74f, 22f, 1f);

    public RectTransform rectTransform { get; private set; }
    public TextMeshProUGUI labelTMP, leftButtonTMP, rightButtonTMP;

    public Action<bool> onToggle;
    
    public bool state { get; private set; }
    
    private RectTransform optionBox, optionBoxBehind;
    private Vector3 targetPosition, targetScale;
    
    public void SetState(bool newState, bool invokeCallback)
    {
        targetPosition = newState ? leftPosition : rightPosition;
        targetScale = newState ? leftScale : rightScale;
        
        if (invokeCallback && state != newState)
            onToggle?.Invoke(newState);
        
        state = newState;
    }

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        labelTMP = GetComponentInChildren<TextMeshProUGUI>();
        optionBox = (RectTransform) transform.Find("Option Box");
        optionBoxBehind = (RectTransform) transform.Find("Option Box Behind");

        var background = transform.Find("SliderBG");
        var backgroundLocalPosition = background.localPosition;
        backgroundLocalPosition.x += 100;
        background.localPosition = backgroundLocalPosition;

        var labelSizeDelta = labelTMP.rectTransform.sizeDelta;
        labelSizeDelta.x -= 4;
        labelTMP.rectTransform.sizeDelta = labelSizeDelta;
        
        var labelLocalPosition = labelTMP.rectTransform.localPosition;
        labelLocalPosition.x += 100;
        labelTMP.rectTransform.localPosition = labelLocalPosition;
        
        var outline = transform.Find("RawImage");
        var outlineLocalPosition = outline.localPosition;
        outlineLocalPosition.x += 100;
        outline.localPosition = outlineLocalPosition;
        
        var fill = transform.Find("RawImage (1)");
        var fillLocalPosition = fill.localPosition;
        fillLocalPosition.x += 100;
        fill.localPosition = fillLocalPosition;
        
        var separator = transform.Find("RawImage (2)");
        var separatorLocalPosition = separator.localPosition;
        separatorLocalPosition.x += 100;
        separator.localPosition = separatorLocalPosition;
        
        var buttons = GetComponentsInChildren<Button>();
        
        var leftButton = buttons[0];

        var leftButtonLocalPosition = leftButton.transform.localPosition;
        leftButtonLocalPosition.x += 100;
        leftButton.transform.localPosition = leftButtonLocalPosition;
        
        leftButton.onClick = new Button.ButtonClickedEvent();
        leftButton.onClick.AddListener(() => SetState(true, true));
        
        leftButtonTMP = leftButton.GetComponentInChildren<TextMeshProUGUI>();
        
        var rightButton = buttons[1];
        
        var rightButtonLocalPosition = rightButton.transform.localPosition;
        rightButtonLocalPosition.x += 100;
        rightButton.transform.localPosition = rightButtonLocalPosition;
        
        rightButton.onClick = new Button.ButtonClickedEvent();
        rightButton.onClick.AddListener(() => SetState(false, true));
        rightButtonTMP = rightButton.GetComponentInChildren<TextMeshProUGUI>();

        Destroy(GetComponent<MenuTwoOptions>());
    }
    
    private void OnTransformParentChanged()
    {
        foreach (var menuButton in GetComponentsInChildren<MenuButton>())
            REPOReflection.menuButton_ParentPage.SetValue(menuButton, GetComponentInParent<MenuPage>());
    }
    
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