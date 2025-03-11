using System.Collections;
using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOTextScroller : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public int maxCharacters;
    
    public float initialWaitTime = 3;
    public float scrollingSpeedInSecondsPerCharacter = .5f;
    public float endWaitTime = 3;
    
    internal void StartAnimation() => StartCoroutine(Animate());
    
    private IEnumerator Animate()
    {
        while (true)
        {
            textMeshPro.firstVisibleCharacter = 0;
            textMeshPro.maxVisibleCharacters = maxCharacters;
            
            var characterCount = textMeshPro.text.Length;

            yield return new WaitForSeconds(initialWaitTime);
            
            while (textMeshPro.maxVisibleCharacters < characterCount)
            {
                textMeshPro.firstVisibleCharacter++;
                textMeshPro.maxVisibleCharacters++;
                yield return new WaitForSeconds(scrollingSpeedInSecondsPerCharacter);
            }
            
            yield return new WaitForSeconds(endWaitTime);
        }
    }
}