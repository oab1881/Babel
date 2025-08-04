using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FloorStyle", menuName = "Floor/Style")]
public class FloorStyle : ScriptableObject
{
    public List<Sprite> styles;
}
