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
    List<Modifier> mods;
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
        mods= new List<Modifier>();
    }

    public StatsVal(float baseVal, bool canNegative, float maxVal)
    {
        this.baseVal = baseVal;
        this.canNegative = canNegative;
        this.maxVal = maxVal;
        mods= new List<Modifier>();
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

    /// <summary>
    /// Adds the newVal para to current val
    /// </summary>
    /// <param name="newVal"></param>
    public void Add(float newVal)
    {
        int modifier = 1;
        foreach (Modifier mod in mods)
        {
            modifier += mod.ModVal;
        }

        if (maxVal != float.NaN && (baseVal + newVal * modifier) > maxVal) baseVal = maxVal;
        else if (canNegative) baseVal += (newVal * modifier);
        else if ((baseVal + newVal * modifier) < 0) baseVal = 0;
        else baseVal += newVal;
    }

    public void AddModifier(float val)
    {
        //Add in new mod here
    }


    
}
