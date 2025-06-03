using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Floor/FloorStats")]
public class UpgradeType : MonoBehaviour
{

    public string upgradeName;
    public Sprite upgradeSprite;
    public int upgradeTier;
    public int cost;
    public int goldPerSecondBonus;
    //public UpgradeEffectType effectType;
    public List<UpgradeType> nextUpgrades; // Tiered / branching upgrades
    public bool isTerminalUpgrade; // For things like Temple
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
