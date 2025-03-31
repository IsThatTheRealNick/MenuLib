using UnityEngine;
using UnityEngine.UI;

namespace MenuLib.MonoBehaviors;

public sealed class REPOObjectPreview : REPOElement
{
    public GameObject previewObject
    {
        get => _previewObject;
        set => UpdatePreviewObject(value);
    }
    
    public bool previewEnabled
    {
        get => _previewEnabled;
        set => UpdatePreviewEnabled(value);
    }

    private PlayerAvatarMenu playerAvatarMenu;
    private GameObject _previewObject;
    private bool _previewEnabled;
    
    private void Awake()
    {
        rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.pivot = Vector2.right;
        rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(184f, 345f);

        playerAvatarMenu = GetComponentInChildren<PlayerAvatarMenuHover>().playerAvatarMenu;
        var playerAvatarVisuals = playerAvatarMenu.GetComponentInChildren<PlayerAvatarVisuals>();
        
        playerAvatarVisuals.gameObject.SetActive(false);
    }

    private void UpdatePreviewObject(GameObject value)
    {
        if (_previewObject != null) Destroy(_previewObject);
        if (playerAvatarMenu == null) return;
        
        _previewObject = Instantiate(value, playerAvatarMenu.transform);
        // Remove gravity from object if it exists
        if (_previewObject.TryGetComponent(typeof(Rigidbody), out var rigidbody))
        {
            Destroy(rigidbody);
        }
        // Set the object's position and rotation
        _previewObject.transform.localPosition = Vector3.zero;
        _previewObject.transform.localRotation = Quaternion.identity;
        
        // TODO Set the object's scale to fit. Get the bounds of the object. Fit in the bounds of the existing PlayerAvatarVisuals
        // var objectBounds = gameObjectClone.GetComponent<Renderer>().bounds;
        // var avatarBounds = playerAvatarMenu.GetComponentInChildren<PlayerAvatarVisuals>().GetComponent<Renderer>().bounds;
        //
        // float scale = Mathf.Min(avatarBounds.size.x / objectBounds.size.x, avatarBounds.size.y / objectBounds.size.y, avatarBounds.size.z / objectBounds.size.z);
        // gameObjectClone.transform.localScale = Vector3.one * scale;
    }

    private void UpdatePreviewEnabled(bool value)
    {
        _previewEnabled = value;
        if (_previewObject != null) previewObject.SetActive(value);
    }

    private void Start() => rectTransform.GetChild(0).localPosition = Vector3.zero;

    private void OnDestroy()
    {
        if (!playerAvatarMenu)
            return;
        
        if (playerAvatarMenu.cameraAndStuff)
            Destroy(playerAvatarMenu.cameraAndStuff.gameObject);
        
        Destroy(playerAvatarMenu.gameObject);
    }
}
