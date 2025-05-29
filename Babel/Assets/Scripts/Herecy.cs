using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Herecy : MonoBehaviour
{

    [SerializeField]
    Slider slider;

    [SerializeField]
    float speed = 8f;


    // Update is called once per frame
    void Update()
    {
        slider.value = Mathf.Lerp(slider.value, GameManager.herecy, speed * Time.deltaTime);
    }
}
