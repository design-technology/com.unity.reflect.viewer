using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Reflect;
using System.Linq;

public class DataHandler : MonoBehaviour
{
    public Transform CameraPosition;
    public Dictionary<Vector3,float> RadValues = new Dictionary<Vector3, float>();
    public Dictionary<Vector3, float> DFValues = new Dictionary<Vector3, float>();
    public Dictionary<Vector3, Color> colValues = new Dictionary<Vector3, Color>();

    public List<Transform> transformsRad = new List<Transform>();
    public List<Transform> transformsDF = new List<Transform>();
    public List<Transform> transformsCol = new List<Transform>();


    public float closestRadFound;
    public float closestDFFound;
    public Vector3 closestColFound;

    private bool receiving = true;
    private Coroutine receivingCoroutine = null;
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
        //if (go.activeInHierarchy)
        {
            var metadata = go.transform.GetComponent<Metadata>();
            if (metadata)
            {
                var Position = go.GetComponent<SyncObjectBinding>().bounds.center;
                //Debug.Log("Found Metadata comp");
                foreach (var item in metadata.GetParameters())
                {
                    if (item.Key == "rad") { transformsRad.Add(go.transform); RadValues.Add(Position, float.Parse(item.Value.value)); }
                    if (item.Key == "df") { transformsDF.Add(go.transform); DFValues.Add(Position, float.Parse(item.Value.value)); }
                    if (item.Key == "col") {
                        transformsCol.Add(go.transform);
                        var colValuesString = item.Value.value.Split(',');
                        colValues.Add(Position, new Color(float.Parse(colValuesString[0]),float.Parse(colValuesString[1]),float.Parse(colValuesString[2]) ));
                    }
                        //Debug.Log("metadata." + go.name + "[ " + item.Key + " ] = " + item.Value.value);
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
    int getClosestPoint(Vector3[] Checks)
    {
        int bestTarget = -1;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = CameraPosition.position;
        for (int i = 0; i <  Checks.Length;i++)
        {
            var potentialTarget = Checks[i];
            //Vector3 directionToTarget = potentialTarget - currentPosition;
            float dSqrToTarget = Vector3.Distance(potentialTarget,currentPosition);// directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = i;
            }
        }

        return bestTarget;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (receiving) { return; }
        var closestRad = getClosestPoint(RadValues.Select(x=>x.Key).ToArray() );
        var closestDF = getClosestPoint(DFValues.Select(x=>x.Key).ToArray() );
        var closestCol = getClosestPoint(colValues.Select(x=>x.Key).ToArray() );
        if (closestRad!=-1 && RadValues.ElementAt(closestRad).Value != closestRadFound)
        {
            closestRadFound = RadValues.ElementAt(closestRad).Value;
        }
        if (closestDF != -1 && DFValues.ElementAt(closestDF).Value != closestDFFound)
        {
            closestDFFound = DFValues.ElementAt(closestDF).Value;
        }
        if (closestCol != -1 && colValues.ElementAt(closestCol).Key != closestColFound)
        {
            closestColFound = colValues.ElementAt(closestCol).Key;
        }

    }
}
