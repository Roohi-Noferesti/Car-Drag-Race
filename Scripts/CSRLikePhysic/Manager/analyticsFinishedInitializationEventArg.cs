using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class analyticsFinishedInitializationEventArg : EventArgs
{
    public enum Status
    {
        Succeeded,
        Failed
    }
}
