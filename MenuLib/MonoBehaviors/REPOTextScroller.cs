using System.Collections;
using TMPro;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

public sealed class REPOTextScroller : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public int maxCharacters;
        
    public float initialWaitTime = 5, startWaitTime = 3, endWaitTime = 3, scrollingSpeedInSecondsPerCharacter = .5f;

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

    private void Awake()
    {
        if (!textMeshPro)
            textMeshPro = GetComponent<TextMeshProUGUI>();

    }
}