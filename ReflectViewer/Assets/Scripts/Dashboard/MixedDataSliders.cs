using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MixedDataSliders : MonoBehaviour
{
    public DataHandler dataHandler;


    public List<string> dataNames;
    public List<GameObject> sliders;

    public List<float> min = new List<float>();
    List<float> max = new List<float>();
    List<float> curentValue = new List<float>();

    // Start is called before the first frame update
    void Start()
    {


        for (int i = 0; i < dataNames.Count; i++)
        {

            if (dataNames[i] == "random" || dataNames[i] == "ratio")
            {
                this.min.Add(0);
                this.max.Add(100);
                this.curentValue.Add(50);
            }


            this.min.Add(dataHandler.GetMin(dataNames[i]));
            this.min.Add(dataHandler.GetMax(dataNames[i]));
            this.curentValue.Add(dataHandler.GetLocalValue(dataNames[1]));

            sliders[i].GetComponentInChildren<Text>().text = dataNames[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < dataNames.Count; i++)
        {

            if (dataNames[i] == "random" || dataNames[i] == "ratio")
            {

                this.curentValue[i] = curentValue[i] + Random.Range(-10, 10);

                if (this.curentValue[i] < min[i])  { this.curentValue[i] = max[i]; }
                if (this.curentValue[i] > max[i])  { this.curentValue[i] = min[i]; }

            }

            this.min.Add(dataHandler.GetMin(dataNames[i]));
            this.min.Add(dataHandler.GetMax(dataNames[i]));
            this.curentValue.Add(dataHandler.GetLocalValue(dataNames[1]));
        }

        for (int i = 0; i < dataNames.Count; i++)
        {
            sliders[i].GetComponent<UnityEngine.UI.Slider>().value = curentValue[i] - min[i] / max[i] - min[i];
        }

    }
}
