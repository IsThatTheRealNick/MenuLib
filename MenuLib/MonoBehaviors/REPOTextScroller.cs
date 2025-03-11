using System.Collections;
using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOTextScroller : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public int maxCharacters;
    
    public float initialWaitTime = 3;
    public float scrollingCharacterPerSecond = .5f;
    public float endWaitTime = 3;

    private bool isInitialRun;
    
    internal void StartAnimation() => StartCoroutine(Animate());
    
    private IEnumerator Animate()
    {
        while (true)
        {
            textMeshPro.firstVisibleCharacter = 0;
            textMeshPro.maxVisibleCharacters = maxCharacters;
            
            var characterCount = textMeshPro.text.Length;

            if (isInitialRun)
            {
                yield return new WaitForSecondsRealtime(initialWaitTime);
                isInitialRun = false;
            }
            else
                yield return new WaitForSecondsRealtime(scrollingCharacterPerSecond);
            
            while (textMeshPro.maxVisibleCharacters < characterCount)
            {
                textMeshPro.firstVisibleCharacter++;
                textMeshPro.maxVisibleCharacters++;
                yield return new WaitForSecondsRealtime(scrollingCharacterPerSecond);
            }
            
            yield return new WaitForSecondsRealtime(endWaitTime);
        }
    }
}