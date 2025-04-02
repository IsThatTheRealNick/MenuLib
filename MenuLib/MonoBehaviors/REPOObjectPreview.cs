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
        // Remove gravity from object if it exists
        foreach (var comp in _previewObject.GetComponents<Component>())
        {
            if (comp is not Transform)
            {
                Destroy(comp);
            }
        }

        // Fix for clown
        var physGrabObjectGrabArea = _previewObject.GetComponentInChildren<PhysGrabObjectGrabArea>();
        if (physGrabObjectGrabArea != null) Destroy(physGrabObjectGrabArea.gameObject);
        
        // Set the object's position and rotation
        _previewObject.transform.localRotation = Quaternion.identity;
        
        var bounds = CalculateCombinedBounds(_previewObject);
        if (bounds != null) CenterObject(_previewObject, bounds.Value);
        //if (bounds != null) ScaleContainerToFitCamera(_previewObject, bounds.Value, playerAvatarMenu.cameraAndStuff.GetComponentInChildren<Camera>());

        // Set the object's scale to fit. Get the bounds of the object. Fit in the bounds of the existing PlayerAvatarVisuals
        //var avatarBounds = GameObject.Find("PlayerAvatarMenu/Player Visuals")?.GetComponentInChildren<Renderer>()?.bounds;
        // Default avatar bounds (0.21, 0.67, 0.21)
        // float scale = Mathf.Min(0.21f / objectBounds.Value.size.x,
        //     0.67f / objectBounds.Value.size.y,
        //     0.21f / objectBounds.Value.size.z);
        // //if (scale > 1) return; // Don't increase size
        // _previewObject.transform.localScale = Vector3.one * scale;
    }
    
    Bounds? CalculateCombinedBounds(GameObject modelInstance)
    {
        Renderer[] renderers = modelInstance.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return null; // No renderers, no bounds
        }

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            // Only include enabled renderers in bounds calculation
            if(renderers[i].enabled)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }
        }
        return combinedBounds;
    }
    
    void CenterObject(GameObject objectInstance, Bounds combinedBounds)
    {
        // Center offset calculation remains the same
        Vector3 worldCenter = combinedBounds.center;
        // We want to find where the pivot *should* be relative to the world center
        // But apply it locally relative to the parent
        Vector3 centerOffsetInLocalSpace = objectInstance.transform.parent.InverseTransformPoint(worldCenter)
                                           - objectInstance.transform.localPosition;

        objectInstance.transform.localPosition = -centerOffsetInLocalSpace;

        // Flip only the Z depth. For some reason the model is too far off axis and this gets it really close. 
        objectInstance.transform.localRotation = objectInstance.transform.localRotation with
        {
            z = -objectInstance.transform.localRotation.z
        };

        // Optional: Log the adjustment for debugging
        Debug.Log($"Model centered. Applied localPosition: {objectInstance.transform.localPosition}");
    }
    
    void ScaleContainerToFitCamera(GameObject container, Bounds modelBounds, Camera camera)
    {
        float distance;
        float frustumHeight;
        float frustumWidth;

        // Calculate frustum size at the distance of the model's center
        if (camera.orthographic)
        {
            frustumHeight = camera.orthographicSize * 2.0f;
            // Orthographic size is half the height, so multiply by 2
        }
        else // Perspective
        {
            // Use the distance from camera plane to bounds center
            Vector3 directionToCenter = modelBounds.center - camera.transform.position;
            distance = Vector3.Dot(directionToCenter, camera.transform.forward); // Project onto camera's forward axis

            if (distance <= 0)
            {
                Debug.LogWarning("Cannot scale model, it is behind or at the camera's position.", container);
                return;
            }
            frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        frustumWidth = frustumHeight * camera.aspect;

        if (frustumHeight <= 0 || frustumWidth <= 0)
        {
            Debug.LogWarning($"Cannot scale model, calculated frustum size is zero or negative (Height: {frustumHeight}, Width: {frustumWidth}).", container);
            return;
        }

        // Get the size of the model's bounds
        Vector3 modelSize = modelBounds.size;

        if (modelSize.x <= 0 && modelSize.y <= 0 && modelSize.z <= 0)
        {
            Debug.LogWarning("Cannot scale model, model bounds size is zero or negative.", container);
            return;
        }

        // Calculate how much larger the model is than the view frustum at its distance
        float scaleFactorWidth = (frustumWidth > 0) ? modelSize.x / frustumWidth : 0f;
        float scaleFactorHeight = (frustumHeight > 0) ? modelSize.y / frustumHeight : 0f;
        float scaleFactorDepth = (frustumWidth > 0) ? modelSize.z / frustumHeight : 0f;

        // We need to scale down by the LARGER factor to ensure both dimensions fit
        float requiredScaleFactor = Mathf.Max(scaleFactorWidth, scaleFactorHeight, scaleFactorDepth);

        if (requiredScaleFactor <= 0)
        {
             // This might happen if model size is zero or frustum was calculated as zero
             Debug.LogWarning("Cannot scale model, required scale factor is zero or negative. Check model bounds and camera setup.", container);
             return;
        }

        // Apply the inverse scale to the container, including padding
        // Dividing the current scale by the required factor effectively scales the object down
        container.transform.localScale = container.transform.localScale / requiredScaleFactor * 0.9f;

        // Debug.Log($"Scaled container '{container.name}'. Model Size: {modelSize}, Frustum Size @ Distance: ({frustumWidth}, {frustumHeight}), Required Shrink Factor: {requiredScaleFactor}, Final Container Scale: {container.transform.localScale}");
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
