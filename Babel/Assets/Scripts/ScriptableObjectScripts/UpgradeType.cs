using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum floorType { baseFloor, archer, temple}

[CreateAssetMenu(menuName = "Floor/UpgradeTypes")]
public class UpgradeType : ScriptableObject
{

    public string upgradeName;
    public Sprite upgradeSprite;
    public int upgradeTier;

    //We could eventually change this to just be cost...
    //By using the nextUpgrade list below we can get the cost for 
    //every item in the list
    public int nextUpgradecost;


    public int goldPerSecondBonus;
    public int herecyPerSecond;
    public floorType currectType;
    public int dps;
    public int attackRange;

    

    //Add in area for custom buttons

    //public UpgradeEffectType effectType;
    public List<UpgradeType> nextUpgrades; // Tiered / branching upgrades
}
