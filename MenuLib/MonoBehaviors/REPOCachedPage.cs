using UnityEngine;

namespace MenuLib.MonoBehaviors;

internal sealed class REPOCachedPage : MonoBehaviour
{
    private MenuPage menuPage;
    
    private void Awake() => MenuAPI.cachedMenuPages.Add(menuPage = GetComponent<MenuPage>());

    private void OnDestroy() => MenuAPI.cachedMenuPages.Remove(menuPage);
}