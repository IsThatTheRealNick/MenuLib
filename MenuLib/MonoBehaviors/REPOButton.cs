using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuLib.MonoBehaviors;

public sealed class REPOButton : MonoBehaviour
{
    public RectTransform rectTransform;
    
    public Button button;
    public MenuButton menuButton;

    public TextMeshProUGUI labelTMP;

    private string currentText;
    
    private void Awake()
    {
        rectTransform = transform as RectTransform;
        button = GetComponent<Button>();
        menuButton = GetComponent<MenuButton>();
        labelTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (labelTMP.text == currentText)
            return;

        rectTransform.sizeDelta = labelTMP.GetPreferredValues();
        currentText = labelTMP.text;
    }

    private void OnTransformParentChanged() => REPOReflection.menuButton_ParentPage.SetValue(menuButton, GetComponentInParent<MenuPage>());
}