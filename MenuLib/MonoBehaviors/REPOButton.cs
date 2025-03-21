using MenuLib.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuLib.MonoBehaviors;

public sealed class REPOButton : MonoBehaviour, IREPOElement
{
    public RectTransform rectTransform { get; private set; }
    
    public Button button;
    public MenuButton menuButton;

    public TextMeshProUGUI labelTMP;

    private string currentText;
    
    public Vector2 GetLabelSize() => labelTMP.GetPreferredValues();
    
    private void Awake()
    {
        rectTransform = transform as RectTransform;
        button = GetComponent<Button>();
        menuButton = GetComponent<MenuButton>();
        labelTMP = GetComponentInChildren<TextMeshProUGUI>();
        
        button.onClick = new Button.ButtonClickedEvent();
        Destroy(GetComponent<MenuButtonPopUp>());
    }

    private void Update()
    {
        if (labelTMP.text == currentText)
            return;

        rectTransform.sizeDelta = GetPreferredTextSize();
        currentText = labelTMP.text;
    }
    
    public Vector2 GetPreferredTextSize() => labelTMP.GetPreferredValues(); 

    private void OnTransformParentChanged() => REPOReflection.menuButton_ParentPage.SetValue(menuButton, GetComponentInParent<MenuPage>());
}