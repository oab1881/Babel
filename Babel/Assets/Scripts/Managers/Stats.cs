using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    private StatsVal money;
    private StatsVal heresy;
    private StatsVal workerCount;
    private StatsVal engineerCount;
    public StatsVal dogma;


    private void Awake()
    {
        money = new StatsVal(0, false);    
        heresy = new StatsVal(0, false, 100);
        engineerCount = new StatsVal(0, false);
        workerCount= new StatsVal(0, false);
    }



    public StatsVal Money
    {
        get { return money; }
        set { money = value; }
    }

    public StatsVal Heresy
    {
        get { return heresy; }
        set { heresy = value; }
    }



    public StatsVal WorkerCount
    {
        get { return workerCount; }
        set { workerCount = value; }
    }
    public StatsVal EngineerCount
    {
        get { return engineerCount; }
        set { engineerCount = value; }
    }


    public void ResetStats()
    {
        money.Modify(0);
        heresy.Modify(0);
        workerCount.Modify(0);
        engineerCount.Modify(0);
        dogma.Modify(0);
    }
}
