using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectType
{
    Money,
    Heresy,
    HeresyAMin
}


//Wrapper is neccary for deserialazation of json
[System.Serializable]
public class EventsWrapper
{
    public List<Events> events;
}



[System.Serializable]
public class Events
{
    public string id;
    public string baseText;
    public string imagePath;
    public List<Choices> choices;
}

[System.Serializable]
public class Choices
{
    public string choiceText;
    public string imageResultPath;
    public Dictionary<EffectType, int> effects;
    public string resultText;
}
