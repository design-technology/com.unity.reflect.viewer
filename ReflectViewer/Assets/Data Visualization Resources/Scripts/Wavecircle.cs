using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wavecircle : MonoBehaviour
{
    public DataHandler dataHandler;

    float min;
    float max;
    float currentValue;
    float averageValue;

    public string dataName;

    public Text progress;
    public Text actualValue;
    public Text title;

    public Transform wave;
    public Transform s, e;


    public string Unit
    {
        get
        {
            if (dataName.Contains("daylight"))
            {
                return "DF";
            }
            else if (dataName.Contains("radiation"))
            {
                return "kWh/m2";
            }
            else
            {
                return "";
            }
        }
    }
   

    void Start()
    {
        min = dataHandler.GetMin(dataName);
        max = dataHandler.GetMax(dataName);
        averageValue = dataHandler.GetAverage(dataName);

        currentValue = dataHandler.GetLocalValue(dataName);
        title.text = dataName;
    }

    // Update is called once per frame
    void Update()
    {
        currentValue = dataHandler.GetLocalValue(dataName);


        float truePercentageValue = (currentValue - min) / (max - min) * 1f;

        wave.position = s.position + (e.position - s.position) * truePercentageValue;
        actualValue.text = Math.Round(currentValue, 2)+ Unit;


        progress.text = Mathf.RoundToInt(currentValue / averageValue * 100) + " %";
    }
}
