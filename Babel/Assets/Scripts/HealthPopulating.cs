using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPopulating : MonoBehaviour
{
    //Creates a list of children
    List<GameObject> children;
    HashSet<GameObject> pendingDestruction = new HashSet<GameObject>();

    //The heart prefab
    [SerializeField]
    GameObject heartPrefab;

    void Update()
    {
        //Every fram we get all the children 
        children = GetAllChildren(gameObject);

        //We test if the health is greater then the children count
        //We then need to populate more hearts
        if (GameManager.health > children.Count)
        {
            AddHearts();
        }

        //If the children count is greater then the total hearts we remove hearts
        else if (GameManager.health < children.Count)
        {
            RemoveHearts();
        }
    }

    //From Unity forums
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

    /// <summary>
    /// Adds new hearts to the UI
    /// </summary>
    void AddHearts()
    {
        for (int i = 0; i < GameManager.health - children.Count; i++)
        {
            Instantiate(heartPrefab, gameObject.transform);
        }
    }

    /// <summary>
    /// Remove hearts from the UI
    /// </summary>
    void RemoveHearts()
    {
        //Figures out how many hearts to remove
        int toRemove = children.Count - GameManager.health;
        int removed = 0;

        //Remove from the beginning (leftmost first)
        for (int i = 0; i < children.Count && removed < toRemove; i++)
        {
            //Gets a reference to the current heart
            GameObject heart = children[i];
            if (!pendingDestruction.Contains(heart))
            {
                pendingDestruction.Add(heart);
                StartCoroutine(FlashAndDestroy(heart));
                removed++;
            }
        }
    }

    IEnumerator FlashAndDestroy(GameObject heart)
    {
        SpriteRenderer sr = heart.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Destroy(heart);
            pendingDestruction.Remove(heart);
            yield break;
        }

        float flashDuration = 1f;
        float flashSpeed = 0.1f;
        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled;

            yield return new WaitForSeconds(flashSpeed);
            elapsed += flashSpeed;
        }

        if (heart != null)
        {
            Destroy(heart);
        }

        pendingDestruction.Remove(heart);
    }
}
