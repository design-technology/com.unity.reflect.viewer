using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleSlider : MonoBehaviour
{
    public DataHandler dataHandler;

    float min;
    float max;
    float currentValue;
    float averageValue;

    public string dataName;


    public Image image;
    public Text progress;
    public Text actualValue;
    public Text title;
    public Text distance;


    void OnEnable()
    {
        min = dataHandler.GetMin(dataName);
        max = dataHandler.GetMax(dataName);
        averageValue = dataHandler.GetAverage(dataName);

        currentValue = dataHandler.GetLocalValue(dataName);
        title.text = dataName;
    }

    void Update()
    {
        currentValue = dataHandler.GetLocalValue(dataName);

        float val = (currentValue - min) / (max - min);
        image.fillAmount = val;

        var perc = (int)((currentValue / averageValue) * 100) + "%";
        progress.text = perc;

        actualValue.text = currentValue + dataHandler.GetUnit(dataName);
    }

}


