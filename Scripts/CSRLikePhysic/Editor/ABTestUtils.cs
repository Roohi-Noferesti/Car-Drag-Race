using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ABTestUtils
{
    public static void ToNextVersion(this SerializedProperty versionprop)
    {
        versionprop.intValue++;
    }

    public static void ToPreviousVersion(this SerializedProperty versionprop)
    {
        versionprop.intValue--;
    }
}
