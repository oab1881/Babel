using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom value as floats that can be modified used in place of normal primitives
/// </summary>
[System.Serializable]
public class StatsVal
{
    float baseVal;
    float modifier = 1f;
    bool canNegative;
    float maxVal = float.NaN;

    public float Val
    {
        get { return baseVal; }
    }

    public StatsVal(float baseVal, bool canNegative)
    {
        this.baseVal = baseVal;
        this.canNegative = canNegative;
    }

    public StatsVal(float baseVal, bool canNegative, float maxVal)
    {
        this.baseVal = baseVal;
        this.canNegative = canNegative;
        this.maxVal = maxVal;
    }
    /// <summary>
    /// Overwrites base value to the new value
    /// </summary>
    /// <param name="newVal">New value to overwrite with</param>
    public void Modify(float newVal)
    {
        if (maxVal != float.NaN && newVal > maxVal) baseVal = maxVal;
        else if (canNegative) baseVal = newVal;
        else if (newVal < 0) baseVal = 0;
        else baseVal = newVal;
    }

    public void Add(float newVal)
    {
        if (maxVal != float.NaN && (baseVal + newVal * modifier) > maxVal) baseVal = maxVal;
        else if (canNegative) baseVal += (newVal * modifier);
        else if ((baseVal + newVal * modifier) < 0) baseVal = 0;
        else baseVal += newVal;
    }

    public void AddModifier(float val)
    {
        modifier += val;
    }

}
