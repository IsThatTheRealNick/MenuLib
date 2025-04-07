using UnityEngine;
using UnityEngine.UI;

namespace MenuLib.MonoBehaviors;

public sealed class REPOObjectPreview : REPOElement
{
    public GameObject previewObject
    {
        get => _previewObject;
        set
        {
            if (_previewObject == value)
                return;
            
            UpdatePreviewGameObject(value);
            _previewObject = value;
        }
    }
    
    public bool enableBackgroundImage
    {
        get => backgroundImage.enabled;
        set => backgroundImage.enabled = value;
    }
    
    public Color backgroundImageColor
    {
        get => backgroundImage.color;
        set => backgroundImage.color = value;
    }
    
    public Vector2 previewSize
    {
        get => rectTransform.sizeDelta;
        set
        {
            const float ASPECT_RATIO = 0.53333336f;
            
            if (value.x > value.y)
                value = value with { y = value.x / ASPECT_RATIO };
            else
                value = value with { x = value.y * ASPECT_RATIO };
            
            renderTextureRectTransform.sizeDelta = rectTransform.sizeDelta = value;
            renderTextureRectTransform.localPosition = Vector3.zero;
        }
    }

    private PlayerAvatarMenuHover playerAvatarMenuHover;
    private Image backgroundImage;
    private RectTransform renderTextureRectTransform;
    
    private GameObject _previewObject;
    
    private void Awake()
    {
        rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.pivot = Vector2.right;
        rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;

        renderTextureRectTransform = (RectTransform) rectTransform.GetChild(1);
        renderTextureRectTransform.localPosition = Vector3.zero;
        
        playerAvatarMenuHover = GetComponentInChildren<PlayerAvatarMenuHover>();
        playerAvatarMenuHover.playerAvatarMenu.cameraAndStuff.GetComponentInChildren<Camera>().farClipPlane = 100f;
        
        backgroundImage = gameObject.AddComponent<Image>();
        backgroundImage.enabled = false;
    }

    private void Start()
    {
        var playerAvatarMenuTransform = playerAvatarMenuHover.playerAvatarMenu.transform;

        for (var i = 0; i < 3; i++)
            Destroy(playerAvatarMenuTransform.GetChild(i).gameObject);
    }

    private void OnDestroy()
    {
        if (!playerAvatarMenuHover || !playerAvatarMenuHover.playerAvatarMenu)
            return;

        var playerAvatar = playerAvatarMenuHover.playerAvatarMenu;
        
        if (!playerAvatar)
            return;
        
        var cameraStuffTransform = playerAvatar.cameraAndStuff;
        
        if (cameraStuffTransform)
            Destroy(cameraStuffTransform.gameObject);
        
        Destroy(playerAvatar.gameObject);
    }

    private void UpdatePreviewGameObject(GameObject previewGameObject)
    {
        if (previewObject)
            Destroy(previewObject);
        
        previewGameObject.transform.SetParent(playerAvatarMenuHover.playerAvatarMenu.transform, false);
        
        if (previewGameObject.GetComponent<Rigidbody>() is { } previewRigidbody)
            previewRigidbody.automaticInertiaTensor = false;
    }
}
