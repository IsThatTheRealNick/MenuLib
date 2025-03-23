using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MenuLib.MonoBehaviors;

// The template this component attatches to does not have a RectTransform by design,
// so this class cannot extend REPOElement.
public sealed class REPOAvatarPreview : MonoBehaviour
{
    public PlayerAvatarMenu playerAvatarMenu { get; private set; }
    public PlayerAvatarMenuHover playerAvatarMenuHover { get; private set; }

    private void Awake()
    {
        // This component holds the camera and
        // moves itself out of the UI and into the root hiearchy.
        playerAvatarMenu = this.GetComponentInChildren<PlayerAvatarMenu>();

        // This component holds the RenderTexture, and stays in the UI hiearchy.
        playerAvatarMenuHover = this.GetComponentInChildren<PlayerAvatarMenuHover>();
    }
}
