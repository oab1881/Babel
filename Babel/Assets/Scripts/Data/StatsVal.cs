using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    /// <summary>
    /// For creating stats
    /// </summary>
    /// <param name="baseVal">The intial value of the stat</param>
    /// <param name="canNegative">True it can be negative, False it can not be negative</param>
    public StatsVal(float baseVal, bool canNegative)
    {
        this.baseVal = baseVal;
        this.canNegative = canNegative;
        mods= new List<Modifier>();

        //Makes the checkmods function call every frame 
        Stats.loop += CheckMods;
    }

    /// <summary>
    /// Overload construcotr for stats with a max value
    /// </summary>
    /// <param name="baseVal"></param>
    /// <param name="canNegative"></param>
    /// <param name="maxVal"></param>
    public StatsVal(float baseVal, bool canNegative, float maxVal) : this(baseVal, canNegative)
    {
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

    /// <summary>
    /// Adds the newVal para to current val
    /// </summary>
    /// <param name="newVal">new value to add to the current val</param>
    public void Add(float newVal)
    {
        //Modifiers base is always 1
        int modifier = 1;

        //Changes the modifier based on all other mods
        foreach (Modifier mod in mods)
        {
            modifier += mod.ModVal;
        }

        //If there is a max val and it is bigger than the max val
        if (maxVal != float.NaN && (baseVal + newVal * modifier) > maxVal) baseVal = maxVal;

        //If it can be negative
        else if (canNegative) baseVal += (newVal * modifier);

        //If it can't be negative and goes under 0
        else if ((baseVal + newVal * modifier) < 0) baseVal = 0;

        //No stipulations can't be negative or has a max val
        else baseVal += newVal * modifier;
    }

    /// <summary>
    /// Creates a modifier for the stat
    /// </summary>
    /// <param name="val">Modifier value</param>
    /// <param name="time">Time it takes for before the modifier goes away</param>
    public void AddModifier(int val, float time)
    {
        mods.Add(new Modifier(val, time));
    }

    /// <summary>
    /// Creates a permenant modifier for the stat
    /// </summary>
    /// <param name="val">Modifier value</param>
    public void AddModifier(int val)
    {
        mods.Add(new Modifier(val));
    }

    /// <summary>
    /// Is attacehd to the stats script loop Action checks to see if they should get rid of the modifier
    /// </summary>
    private void CheckMods()
    {
        for(int i = 0; i < mods.Count; i++)
        {
            if(mods[i].CheckMod())
            {
                mods.RemoveAt(i);
                i++;
            }
        }
    }
}
