using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class JournalContent
{
    public string stringContent = "";
    public Sprite spriteContent = null;

    // padding around the content
    public float verticalPadding = 5;
    public float horizontalPadding = 5;
}
