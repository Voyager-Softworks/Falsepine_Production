using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class JournalContent  /// @todo comment
{
    [TextArea(3, 10)]
    public string text = "";
    public Sprite image = null;

    // padding around the content
    //public float verticalPadding = 5;
    //public float horizontalPadding = 5;
}
