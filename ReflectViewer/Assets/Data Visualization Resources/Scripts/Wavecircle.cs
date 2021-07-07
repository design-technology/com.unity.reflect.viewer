using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wavecircle : MonoBehaviour
{
    public DataHandler datahandler;
    float max = 200;
    float min = 40;

    float currentValue;
    public string dataName;

    public Image image;
    public Text progress;
    public Text actualValue;
    public Text title;

    bool up = true;
    int frameCount = 0;


    [Range(0, 100)]
    public float no1;

    public Transform wave;
    public Transform s, e;


    void Start()
    {
        currentValue = min;
        title.text = dataName;
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCount % 40 == 0)
        {
            UpdateValue();
        }

        frameCount++;
    }

    //void UpdatePercent(float f)
    //{
    //    wave.position = s.position + (e.position - s.position) * f / 100;

    //    theText.text = Mathf.RoundToInt(f) + "%";
    //}

    void AddOne()
    {
        currentValue++;
    }

    void MinusOne()
    {
        currentValue--;
    }


    void UpdateValue()
    {
        currentValue = datahandler.closestDFFound;
        //if (currentValue >= max)
        //{
        //    up = false;
        //}
        //else if (currentValue <= min)
        //{
        //    up = true;
        //}

        //if (up)
        //{
        //    AddOne();
        //}
        //else
        //{
        //    MinusOne();
        //}

        float val = (currentValue - min) / (max - min)*1f;

        wave.position = s.position + (e.position - s.position) * val;
        actualValue.text = currentValue + " DF";
        progress.text = Mathf.RoundToInt(val * 100) + " %";


        //var perc = (int)(image.fillAmount * 100) + "%";
        //progress.text = perc;

        // actualValue.text = currentValue + " dd";
    }
}
