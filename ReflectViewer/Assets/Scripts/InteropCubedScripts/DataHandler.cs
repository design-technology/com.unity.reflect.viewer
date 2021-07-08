using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;
using System.Linq;
using TMPro;

public class DataHandler : MonoBehaviour
{
    public Transform CameraPosition;
    public int SpheresLayer = -1;
    public Dictionary<Vector3, float> RadValues = new Dictionary<Vector3, float>();
    public Dictionary<Vector3, float> DFValues = new Dictionary<Vector3, float>();
    public Dictionary<Vector3, Color> colValues = new Dictionary<Vector3, Color>();

    private float _adf = 0.0f;

    public float AverageDaylightFactor()
    {
        if (_adf != 0.0f) return _adf;
        var averageDaylightFactor = 0f;
        foreach (var item in DFValues)
        {
            averageDaylightFactor += item.Value;
        }
        averageDaylightFactor /= DFValues.Count;
        _adf = averageDaylightFactor;
        return averageDaylightFactor;
    }
    private float _arf = 0.0f;
    public float AverageRadiation()
    {
        if (_arf != 0.0f) return _arf;
        var averageRadiation = 0f;
        foreach (var item in RadValues)
        {
            averageRadiation += item.Value;
        }
        averageRadiation /= RadValues.Count;
        _arf = averageRadiation;
        return averageRadiation;
    }

    public List<Transform> transformsRad = new List<Transform>();
    public List<Transform> transformsDF = new List<Transform>();
    public List<Transform> transformsCol = new List<Transform>();

 
    public float radiationMin = float.MaxValue;
    public float radiationMax = float.MinValue;
    public float daylightMin = float.MaxValue;
    public float daylightMax = float.MinValue;

    public float radiationDistance;
    public float daylightDistance;

    public float closestRadFound;
    public float closestDFFound;
    public Vector3 closestColFound;

    public string daylightName = "daylight";
    public string radName = "radiation";

    public bool receiving = true;
    private Coroutine receivingCoroutine = null;
    private List<GameObject> spheresToDisplay = new List<GameObject>();
#if UNITY_EDITOR
    public TMP_Text Debugger;
#endif
    // Start is called before the first frame update
    void Start()
    {
        SyncObjectBinding.OnCreated += onNewObject;
    }

    void onNewObject(GameObject go)
    {
        receiving = true;
        if (receivingCoroutine != null)
            StopCoroutine(receivingCoroutine);

        // Read object attributes from Rhino layers.
        var metadata = go.transform.GetComponent<Metadata>();
        if (metadata)
        {
            var Position = go.GetComponent<Renderer>().bounds.center;
#if UNITY_EDITOR
            Debug.Log("Found Metadata comp, Gameobject is at position: " + go.transform.position + " bounds.center: " + Position);
            //Debugger.text = ("Found Metadata comp, Gameobject is at position: " + go.transform.position + " bounds.center: " + Position);
#endif
            //if (metadata.GetParameter("Document").Contains("Rhino"))
            foreach (var item in metadata.GetParameters())
            {
                if (item.Key == "Layer")
                {
                    Debug.Log("Found Layer:" + item.Value.value);
                    if (int.Parse(item.Value.value) == SpheresLayer)
                    {
                        spheresToDisplay.Add(go);
                    }
                }
                if (item.Key == "rad")
                {
                    transformsRad.Add(go.transform);
                    var radFloat = float.Parse(item.Value.value);
                    Debug.Log("radFloat:" + radFloat);
                    if (radFloat > radiationMax) radiationMax = radFloat;
                    if (radFloat < radiationMin) radiationMin = radFloat;
                    RadValues.Add(Position, radFloat);
                }
                if (item.Key == "df")
                {
                    transformsDF.Add(go.transform);
                    var DFFloat = float.Parse(item.Value.value);
                    if (DFFloat > daylightMax) daylightMax = DFFloat;
                    if (DFFloat < daylightMin) daylightMin = DFFloat;
                    DFValues.Add(Position, DFFloat);
                }
                if (item.Key == "col")
                {
                    transformsCol.Add(go.transform);
                    var colValuesString = item.Value.value.Split(',');
                    colValues.Add(Position, new Color(float.Parse(colValuesString[0]), float.Parse(colValuesString[1]), float.Parse(colValuesString[2])));
                }
            }
        }
        receivingCoroutine = StartCoroutine(stillreceiving());
    }
    private IEnumerator stillreceiving()
    {
        yield return new WaitForSeconds(3.0f);
        receiving = false;
    }

    int GetClosestPointIndex(Vector3[] Checks)
    {
        int bestTarget = -1;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = CameraPosition.position;

        for (int i = 0; i < Checks.Length; i++)
        {
            var potentialTarget = Checks[i];
            float dSqrToTarget = Vector3.Distance(potentialTarget, currentPosition);
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = i;
            }
        }

        return bestTarget;
    }


    GameObject lastColored = null;
    Color lastColoredoriginal;

    void ColorObject(GameObject go)
    {
        if (lastColored != null)
            lastColored.GetComponent<Renderer>().material.SetColor("_Tint", lastColoredoriginal);

        var renderer = go.GetComponent<Renderer>();
        lastColoredoriginal = renderer.material.GetColor("_Tint");
        renderer.material.SetColor("_Tint", Color.magenta);
        lastColored = go;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (receiving) { return; }

        var closestRad = GetClosestPointIndex(RadValues.Select(x => x.Key).ToArray());

        var closestDF = GetClosestPointIndex(DFValues.Select(x => x.Key).ToArray());

        var closestCol = GetClosestPointIndex(colValues.Select(x => x.Key).ToArray());


        // Update the value if it changed
        if (closestRad != -1 && RadValues.ElementAt(closestRad).Value != closestRadFound)
        {
            closestRadFound = RadValues.ElementAt(closestRad).Value;
            ColorObject(transformsRad[closestRad].gameObject);

            Vector3 currentPosition = CameraPosition.position;
            radiationDistance = Vector3.Distance(RadValues.ElementAt(closestRad).Key, currentPosition);
        }

        if (closestDF != -1 && DFValues.ElementAt(closestDF).Value != closestDFFound)
        {
            closestDFFound = DFValues.ElementAt(closestDF).Value;
            ColorObject(transformsDF[closestDF].gameObject);

            Vector3 currentPosition = CameraPosition.position;
            daylightDistance = Vector3.Distance(DFValues.ElementAt(closestDF).Key, currentPosition);
        }

        if (closestCol != -1 && colValues.ElementAt(closestCol).Key != closestColFound)
        {
            closestColFound = colValues.ElementAt(closestCol).Key;
            ColorObject(transformsCol[closestCol].gameObject);
        }

    }


    #region Dashboard helpers
    public float GetMin(string analysisName)
    {
        if (analysisName.Contains(daylightName))
        {
            return daylightMin;
        }
        else if (analysisName.Contains("radiation"))
        {
            return radiationMin;
        }
        return -1f;
    }

    public float GetMax(string analysisName)
    {
        if (analysisName.Contains(daylightName))
        {
            return daylightMax;
        }
        else if (analysisName.Contains("radiation"))
        {
            return radiationMax;
        }
        return -1f;
    }

    public float GetLocalValue(string analysisName)
    {
        if (analysisName.Contains(daylightName))
        {
            return closestDFFound;
        }
        else if (analysisName.Contains(radName))
        {
            return closestRadFound;
        }
        return -1f;
    }

    public float GetAverage(string analysisName)
    {
        if (analysisName.Contains(daylightName))
        {
            return AverageDaylightFactor();
        }
        else if (analysisName.Contains(radName))
        {
            return AverageRadiation();
        }
        return -1f;
    }

    public string GetUnit(string analysisName)
    {
        if (analysisName.Contains(daylightName))
        {
            return " DF";
        }
        else if (analysisName.Contains(radName))
        {
            return " kWh/m2";
        }
        return "";
    }

    public float GetDistanceToDataPoint(string analysisName)
    {
        if (analysisName.Contains(daylightName))
        {
            return daylightDistance;
        }
        else if (analysisName.Contains(radName))
        {
            return radiationDistance;
        }
        return -1;
    }
    #endregion
}
