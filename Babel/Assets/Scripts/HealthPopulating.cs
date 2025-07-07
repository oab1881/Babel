using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPopulating : MonoBehaviour
{
    List<GameObject> children;

    [SerializeField]
    GameObject heartPrefab;
    

    // Update is called once per frame
    void Update()
    {
        children = GetAllChildren(gameObject);
        if(GameManager.health > children.Count) AddHearts();
        if(GameManager.health < children.Count) RemoveHearts();
    }

    //From forums
    //Goes through the parent getting children recursively
    List<GameObject> GetAllChildren(GameObject obj)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in obj.transform)
        {
            children.Add(child.gameObject);
            children.AddRange(GetAllChildren(child.gameObject));
        }
        return children;
    }

    void AddHearts()
    {
        for(int i = 0; i < GameManager.health - children.Count; i++)
        {
            Instantiate(heartPrefab, gameObject.transform);
        }
    }
    
    void RemoveHearts()
    {
        int decrease = children.Count - GameManager.health;
        for(int i = 0; i < decrease; i++)
        {
            Destroy(children[children.Count - 1]);
        }
    }
}
