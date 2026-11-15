using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CCondition
{
    public abstract bool CheckCondition();
}


[Serializable]
public class CNotUseableCondition : CCondition
{
    public override bool CheckCondition()
    {
        return false;
    }
}
