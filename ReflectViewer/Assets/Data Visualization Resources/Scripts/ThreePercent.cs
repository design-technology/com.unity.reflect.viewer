 
using UnityEngine;
using UnityEngine.UI;

public class ThreePercent : MonoBehaviour
{

    public float solidPanel, glassPanel;

    public Image solidPanelCircle, glassPanelCircle;
 
    public Text ratio1, ratio2;

    // Start is called before the first frame update
    void Start()
    {
        solidPanel = 0.75f;
        glassPanel = 0.25f;
        UpdatePercent(solidPanel, glassPanel);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePercent(no1,no2);
    }
    public void UpdatePercent(float n1,float n2)
    {
        float sum = n1 + n2;
        float p1 = n1 / sum;
        float p2 = n2 / sum;


        solidPanelCircle.fillAmount = p1 - 0.02f;
        glassPanelCircle.fillAmount = p2 - 0.02f;
        glassPanelCircle.transform.localEulerAngles = -new Vector3(0, 0, 360 * p1);




        ratio1.text = (int)(p1 * 100) + "%";
        ratio2.text = (int)(p2 * 100) + "%";

    }
}
