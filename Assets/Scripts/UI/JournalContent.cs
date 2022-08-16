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

    // equality operator
    public static bool operator ==(JournalContent lhs, JournalContent rhs)
    {
        if (System.Object.ReferenceEquals(lhs, rhs))
        {
            return true;
        }

        if (((object)lhs == null) || ((object)rhs == null))
        {
            return false;
        }
        return lhs.text == rhs.text && lhs.image == rhs.image;
    }
    //inequality operator
    public static bool operator !=(JournalContent lhs, JournalContent rhs)
    {
        return !(lhs == rhs);
    }
    //override equals method
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        JournalContent other = obj as JournalContent;
        if (other == null)
        {
            return false;
        }
        return this == other;
    }
}
