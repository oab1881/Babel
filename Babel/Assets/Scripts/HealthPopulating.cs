using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPopulating : MonoBehaviour
{
    List<GameObject> children;
    HashSet<GameObject> pendingDestruction = new HashSet<GameObject>();

    [SerializeField]
    GameObject heartPrefab;

    void Update()
    {
        children = GetAllChildren(gameObject);
        if (GameManager.health > children.Count)
        {
            AddHearts();
        }
        else if (GameManager.health < children.Count)
        {
            RemoveHearts();
        }
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
        for (int i = 0; i < GameManager.health - children.Count; i++)
        {
            Instantiate(heartPrefab, gameObject.transform);
        }
    }

    void RemoveHearts()
    {
        int toRemove = children.Count - GameManager.health;
        int removed = 0;

        //Remove from the beginning (leftmost first)
        for (int i = 0; i < children.Count && removed < toRemove; i++)
        {
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
