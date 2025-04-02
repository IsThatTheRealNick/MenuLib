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
    private GameObject parentObject;
    private GameObject _previewObject;
    private bool _previewEnabled;
    
    private void Awake()
    {
        rectTransform = gameObject.AddComponent<RectTransform>();
        rectTransform.pivot = Vector2.right;
        rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(184f, 345f);

        playerAvatarMenu = GetComponentInChildren<PlayerAvatarMenuHover>().playerAvatarMenu;
        playerAvatarMenu.cameraAndStuff.GetComponentInChildren<Camera>().farClipPlane = 100;
        parentObject = new GameObject("Preview Object Parent");
        parentObject.transform.SetParent(playerAvatarMenu.transform, false);
        var playerAvatarVisuals = playerAvatarMenu.GetComponentInChildren<PlayerAvatarVisuals>();
        
        // Disable fast spinning (typical on small objects), causing it to rotate on other axis unexpectedly.
        var component = playerAvatarMenu.GetComponent<Rigidbody>();
        if (component != null) component.automaticInertiaTensor = false;
        
        playerAvatarVisuals.gameObject.SetActive(false);
    }

    private void UpdatePreviewObject(GameObject value)
    {
        if (_previewObject != null) Destroy(_previewObject);
        if (playerAvatarMenu == null) return;
        
        _previewObject = Instantiate(value, parentObject.transform);
        
        _previewObject.SetActive(previewEnabled);
        Vector3? center = null;
        // Remove gravity from object if it exists
        foreach (var comp in _previewObject.GetComponents<Component>())
        {
            if (comp is Rigidbody rbTemp)
            {
                center = rbTemp.centerOfMass;
                rbTemp.useGravity = false;
                rbTemp.isKinematic = true;
            }
            if (comp is not Transform)
            {
                Destroy(comp);
            }
        }
        
        if (!center.HasValue) {
            // Use rigidbody to get the center (of mass) of the object
            var rb = _previewObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            center = rb.centerOfMass;
            
            // Dispose of rigidbody after getting the center
            Destroy(rb);
        }

        // Fix for clown
        var physGrabObjectGrabArea = _previewObject.GetComponentInChildren<PhysGrabObjectGrabArea>();
        if (physGrabObjectGrabArea != null) Destroy(physGrabObjectGrabArea.gameObject);
        
        // Set the object's position and rotation
        _previewObject.transform.localRotation = Quaternion.identity;
        _previewObject.transform.localPosition = -center.Value;
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
