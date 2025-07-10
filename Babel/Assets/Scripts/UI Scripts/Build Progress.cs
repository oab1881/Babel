using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildProgress : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    [SerializeField]
    float speed = 8f;

    [SerializeField]
    Clicker clickManager;

    // Update is called once per frame
    void Update()
    {
        slider.maxValue = clickManager.ClickRequirement;
        slider.value = Mathf.Lerp(slider.value, clickManager.CurrentClickProgress, speed * Time.deltaTime);
    }
}
