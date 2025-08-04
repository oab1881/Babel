using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum floorType { baseFloor, archer, temple}

[CreateAssetMenu(menuName = "Floor/UpgradeTypes")]
public class UpgradeType : ScriptableObject
{
    [Header("Base Info")]
    public string upgradeName;
    public int upgradeTier;
    public floorType currectType;

    //We could eventually change this to just be cost...
    //By using the nextUpgrade list below we can get the cost for 
    //every item in the list
    public int nextUpgradecost;

    [Tooltip("Leave this empty unless there are new style sets that need to be picked. For example from Base type to Archer typer or Base to Temple.")]
    public List<FloorStyle> floorStyle; //Leave exmpty to not reset floor type

    

    [Header("Stat Changes")]
    public int goldPerSecond;
    public int herecyChange;
    public int herecyPerSecond;

    //Not integrated yet
    public int populationChange;

    [Header("Attack")]
    public float dps;
    public float attackRange;

    [Header("Upgrade")]
    public ButtonStyles buttons;
    public List<UpgradeType> nextUpgrades; // Tiered / branching upgrades

    
}
