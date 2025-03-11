using System.Collections;
using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOTextScroller : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public int maxCharacters;
    
    public float initialWaitTime = 5;
    public float startWaitTime = 3;
    public float scrollingSpeedInSecondsPerCharacter = .5f;
    public float endWaitTime = 3;

    private bool isInitial = true;
    
    public IEnumerator Animate()
    {
        while (true)
        {
            textMeshPro.firstVisibleCharacter = 0;
            textMeshPro.maxVisibleCharacters = maxCharacters;

            if (isInitial)
            {
                yield return new WaitForSeconds(initialWaitTime);
                isInitial = false;
            }
            else
                yield return new WaitForSeconds(startWaitTime);
            
            while (textMeshPro.maxVisibleCharacters < textMeshPro.text.Length)
            {
                textMeshPro.firstVisibleCharacter++;
                textMeshPro.maxVisibleCharacters++;
                yield return new WaitForSeconds(scrollingSpeedInSecondsPerCharacter);
            }
            
            yield return new WaitForSeconds(endWaitTime);
        }
    }
}