using System;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace MenuLib.MonoBehaviors;

public sealed class REPOToggle : MonoBehaviour
{
    private static readonly Vector3 leftPosition = new(37.8f, 12.3f), leftScale = new(73f, 22f, 1f), rightPosition = new(112.644f, 12.3f), rightScale = new(74f, 22f, 1f);
    
    public TextMeshProUGUI labelTMP, leftButtonTMP, rightButtonTMP;

    public Action<bool> onToggle;
    
    public bool state { get; private set; }
    
    private RectTransform optionBox, optionBoxBehind;
    private Vector3 targetPosition, targetScale;

    private void Awake()
    {
        labelTMP = GetComponentInChildren<TextMeshProUGUI>();
        optionBox = (RectTransform) transform.Find("Option Box");
        optionBoxBehind = (RectTransform) transform.Find("Option Box Behind");
        
        var buttons = GetComponentsInChildren<Button>();
        
        var leftButton = buttons[0];
        leftButton.onClick = new Button.ButtonClickedEvent();
        leftButton.onClick.AddListener(() => SetState(true, true));
        
        leftButtonTMP = leftButton.GetComponentInChildren<TextMeshProUGUI>();
        
        var rightButton = buttons[1];
        rightButton.onClick = new Button.ButtonClickedEvent();
        rightButton.onClick.AddListener(() => SetState(false, true));
        rightButtonTMP = rightButton.GetComponentInChildren<TextMeshProUGUI>();

        Destroy(GetComponent<MenuTwoOptions>());
    }
    
    public void SetState(bool newState, bool invokeCallback)
    {
        if (state == newState)
            return;
        
        targetPosition = newState ? leftPosition : rightPosition;
        targetScale = newState ? leftScale : rightScale;
        
        if (invokeCallback)
            onToggle?.Invoke(newState);
        
        state = newState;
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