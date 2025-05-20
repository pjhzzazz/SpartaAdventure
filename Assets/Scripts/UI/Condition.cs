using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float StartValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;
    
    void Start()
    {
        curValue = StartValue;
    }

    void Update()
    {
        uiBar.fillAmount = GetPercentage();
    }
    
    float GetPercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue += value;
        if (curValue > maxValue)
        {
            curValue = maxValue;
        }
    }

    public void Subtract(float value)
    {
        curValue -= value;
        if (curValue < 0)
        {
            curValue = 0;
        }
    }
}
