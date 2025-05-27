using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPopup : MonoBehaviour
{
    public float lifetime = 1.5f;
    public Vector3 floatOffset = new Vector3(0.5f, 1.0f, 0);
    public float floatSpeed = 1.0f;

    private Vector3 targetPosition;
    private float timer = 0f;

    void Start()
    {
        targetPosition = transform.position + floatOffset;
    }

    void Update()
    {
        // Smooth float
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * floatSpeed);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}

