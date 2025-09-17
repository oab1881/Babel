using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modifier
{
    private int modVal;
    private float timeTotal = float.NaN;
    private float timePassed = float.NaN;
    private bool peremenant = false;

    public int ModVal
    {
        get { return modVal; }
    }


    /// <summary>
    /// Default timed modifier
    /// </summary>
    /// <param name="modVal">The value of the modifier</param>
    /// <param name="timeTotal">Total time of the modifier in mili seconds</param>
    public Modifier(int modVal, float timeTotal)
    {
        this.modVal = modVal;
        this.timeTotal = timeTotal;
        timePassed = 0.0f;
        peremenant= false;
    }


    /// <summary>
    /// Overload constructor for permanent modifiers
    /// </summary>
    /// <param name="modVal">The value of the modifier</param>
    public Modifier(int modVal)
    {
        this.modVal = modVal;
        peremenant= true;
    }


    /// <summary>
    /// Checks if the modifier has expired
    /// </summary>
    /// <returns>Returns false if permenent; Returns false if the timer isn't greater or equal to total time; Returns true if this is true</returns>
    public bool CheckMod()
    {
        if(peremenant) return false;
        timePassed += Time.deltaTime;
        if (timePassed >= timeTotal) return true;
        return false;
    }
}
