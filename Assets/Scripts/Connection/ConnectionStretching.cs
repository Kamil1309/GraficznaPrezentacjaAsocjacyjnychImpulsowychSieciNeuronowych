using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MathHelp;

public class ConnectionStretching : MonoBehaviour
{
    private IEnumerator expandCoroutine;
    private IEnumerator shrinkCoroutine;

    [SerializeField] float toVelocity = 0.06f;
    float fromVelocityMultiplier;

    [SerializeField] float percentFrom = 0.7f;
    float percentTo;

    [HideInInspector] public float distFromMid;

    [HideInInspector] public Tuple<Vector3, Vector3> dendritePosOutside;
    [HideInInspector] public Tuple<Vector3, Vector3> dendritePosInside;

    [HideInInspector]public bool isExpansionFinished = true;
    [HideInInspector]public bool isShrinkingFinished = true;

    Vector3 dirFromTo;
    Vector3 dirToFrom;

    [HideInInspector] public bool connectionInProgress = false;

    Connection connection;

    public float actualSimulationSpeed = 1.0f;
    [HideInInspector] public GameObject simulationTimePanel;

    public void InitStreatchingData(){
        connection = GetComponent<Connection>();

        percentTo = 1 - percentFrom;
        fromVelocityMultiplier = percentFrom/percentTo;
        distFromMid = 5/4f * connection.synapseImageSize/2f;

        CalculateDendrites();
        
        connection.SetSynapsValue(UnityEngine.Random.Range(0f, 100.0f));

        SetDendritesPosition(dendritePosOutside, dendritePosOutside);
    }

    public void ExpandConnection(){
        isExpansionFinished = false;
        SetConnectionsAreInProgress();

        expandCoroutine = ExpandDendrite();
        StartCoroutine(expandCoroutine);
    }

    public void ShrinkConnection(){
        if(!isExpansionFinished){
            isExpansionFinished = true;
            StopCoroutine(expandCoroutine);
        }

        isShrinkingFinished = false;
        SetConnectionsAreInProgress();

        shrinkCoroutine = ShrinkDendrite();
        StartCoroutine(shrinkCoroutine);
    }

    /// <summary>method <c>ExpandDendrite</c> 
    /// is responsible for the animation of 'establishing a connection' between neurons (expanding of dendrites).
    /// </summary>
    private IEnumerator ExpandDendrite()
    {
        Vector3 fromShift = dirFromTo * toVelocity * percentFrom/percentTo * actualSimulationSpeed;
        Vector3 toShift = dirToFrom * toVelocity * actualSimulationSpeed;

        while(true)
        {
            isExpansionFinished = LengthOfDendrites(fromShift, toShift, true);

            if(!isExpansionFinished){
                yield return new WaitForFixedUpdate();
            }else{
                //Set end position of dendrites and synapses
                SetDendritesPositionInside();
                MergeIfTwoSided(true);
                
                SetConnectionsAreNotInProgress();
                yield break;
            }
        }
    }

    /// <summary>method <c>ShrinkDendrite</c> 
    /// is responsible for the animation of 'dissolving  a connection' between neurons (shrinking of dendrites).
    /// </summary>
    private IEnumerator ShrinkDendrite()
    {
        SplitIfTwoSided();

        Vector3 fromShift = -dirFromTo * toVelocity * fromVelocityMultiplier * actualSimulationSpeed;
        Vector3 toShift = -dirToFrom * toVelocity * actualSimulationSpeed;

        while (true)
        {
            isShrinkingFinished = LengthOfDendrites(fromShift, toShift, false);

            if(!isShrinkingFinished){
                yield return new WaitForFixedUpdate();
            }else{
                SetConnectionsAreNotInProgress();
                
                yield break;
            }
        }
    }

    Vector3 CountSynapseShift(Vector3 dirVec, float R){
        if(dirVec.x == 0 && dirVec.z == 0){
            return new Vector3(0, 0, 0);
        }

        bool reversed = false;

        if(dirVec.z == 0){
            reversed = true;

            dirVec = new Vector3(dirVec.z, dirVec.y, dirVec.x);
        }

        float factor = Mathf.Abs(dirVec.x/dirVec.z);

        float deltaZ = Mathf.Sign(dirVec.z) * R/(Mathf.Sqrt(factor*factor + 1));
        float deltaX = Mathf.Sign(dirVec.x) * factor * R/(Mathf.Sqrt(factor*factor + 1));
        
        if(reversed){
            return new Vector3(deltaZ, 0, deltaX);
        }else{
            return new Vector3(deltaX, 0, deltaZ);
        }
    }

    /// <summary>method <c>LengthOfDendrites</c> 
    /// is responsible for changing size of dendrites while 'initializing' or 'deinitializing' dendrite connection between neurons.
    /// </summary>
    /// <param name="fromShift"> The distance that the end of the dendrite (coming out of the 'From' neuron) will be moved in each iteration.</param>
    /// <param name="toShift"> The distance that the end of the dendrite (coming out of the 'To' neuron) will be moved in each iteration.</param>
    /// <param name="expand"> If its 'true' then dendrite will expand, if 'false' then dendrite will shrink.</param>
    /// <returns> 'true' if expanding/shrinking of dendrites is has ended, otherwise returns 'false'. </returns>
    public bool LengthOfDendrites(Vector3 fromShift, Vector3 toShift, bool expand = true){
        LineRenderer lineFromRend = connection.LineFrom.GetComponent<LineRenderer>();
        LineRenderer lineToRend = connection.LineTo.GetComponent<LineRenderer>();
        
        SetDendritesPositionInside( Tuple.Create<Vector3, Vector3>(lineFromRend.GetPosition(1) + fromShift, lineToRend.GetPosition(1) + toShift) );

        Vector3 lineFromWithoutY = new Vector3(lineFromRend.GetPosition(1).x, 0f, lineFromRend.GetPosition(1).z);
        Vector3 lineToWithoutY = new Vector3(lineToRend.GetPosition(1).x, 0f, lineToRend.GetPosition(1).z);

        float lenBetweenEnds= (lineFromWithoutY - lineToWithoutY).magnitude;

        if(expand){
            if(lenBetweenEnds <= distFromMid * 2.2f){
                return true;
            }
        }else{
            float connectionLength = (dendritePosOutside.Item1 - dendritePosOutside.Item2).magnitude;
            if(lenBetweenEnds > 0.95f * connectionLength){
                return true;
            }
        }
        return false;
    }

    /// <summary>method <c>CalcDendritesEndsOutside</c> 
    /// based on "connection.neuronFrom", "connection.neuronTo' which are set for the neuron it calculates outside ends of connection
    /// </summary>
    public void CalcDendritesEndsOutside(){
        Vector3 neuronPos1 = connection.neuronFrom.transform.position;
        Vector3 neuronPos2 = connection.neuronTo.transform.position;
        
        bool reversed = false;

        float x1 = Mathf.Round(neuronPos1.x * 10f) / 10f;
        float z1 = Mathf.Round(neuronPos1.z * 10f) / 10f;
        float x2 = Mathf.Round(neuronPos2.x * 10f) / 10f;
        float z2 = Mathf.Round(neuronPos2.z * 10f) / 10f;
        
        LineEquation line = new LineEquation();
        line.CountCoefficients(x1, z1, x2, z2);
        
        if(float.IsInfinity(line.a)){
            reversed = true;

            x1 = neuronPos1.z;
            z1 = neuronPos1.x;
            x2 = neuronPos2.z;
            z2 = neuronPos2.x;

            line.CountCoefficients(x1, z1, x2, z2);
        }
    
        float R = FindObjectOfType<Neuron>().radius;

        float a1 = 1 + line.a * line.a;
        float b1 = -2 * x1 + 2 * line.a * line.b - 2 * line.a * z1;
        float c1 = x1 * x1 + z1 * z1 + line.b * line.b - 2 * line.b * z1 - R * R;

        var primes1 = QuadraticEquation.SolveQuadratic(a1, b1, c1);

        float a2 = 1 + line.a * line.a;
        float b2 = -2 * x2 + 2 * line.a * line.b - 2 * line.a * z2;
        float c2 = x2 * x2 + z2 * z2 + line.b * line.b - 2 * line.b * z2 - R * R;
        
        var primes2 = QuadraticEquation.SolveQuadratic(a2, b2, c2);

        if(Single.IsNaN( primes1.Item1 )){
            Debug.LogError("ERROR, quadratic equasion (1) has no solution");
        }
        if(Single.IsNaN( primes2.Item1 )){
            Debug.Log(neuronPos1 + "   " + neuronPos2);
            Debug.LogError("ERROR, quadratic equasion (2) has no solution");
        }

        List<float> xList1 = new List<float>();
        List<float> zList1 = new List<float>();

        List<float> xList2 = new List<float>();
        List<float> zList2 = new List<float>();

        xList1.Add(primes1.Item1);
        zList1.Add(line.CountY(primes1.Item1));
        xList2.Add(primes2.Item1);
        zList2.Add(line.CountY(primes2.Item1));

        if(!Single.IsNaN( primes1.Item2 )){
            xList1.Add(primes1.Item2);
            zList1.Add(line.CountY(primes1.Item2));
        }
        if(!Single.IsNaN( primes2.Item2 )){
            xList2.Add(primes2.Item2);
            zList2.Add(line.CountY(primes2.Item2));
        }

        float minDist = float.MaxValue;
        int choosen1 = 0;
        int choosen2 = 0;

        for(int i = 0; i < xList1.Count; i++){
            for(int j = 0; j < xList2.Count; j++){
                float distTemp = (new Vector2(xList1[i], zList1[i]) - new Vector2(xList2[j], zList2[j])).magnitude;
                
                if(distTemp < minDist){
                    minDist = distTemp;
                    choosen1 = i;
                    choosen2 = j;
                }
            }
        }

        Vector3 resultEnd1;
        Vector3 resultEnd2;

        if(!reversed){
            resultEnd1 = new Vector3(xList1[choosen1], neuronPos1.y, zList1[choosen1]);
            resultEnd2 = new Vector3(xList2[choosen2], neuronPos2.y, zList2[choosen2]);
        }else{
            resultEnd1 = new Vector3(zList1[choosen1], neuronPos1.y, xList1[choosen1]);
            resultEnd2 = new Vector3(zList2[choosen2], neuronPos2.y, xList2[choosen2]);
        }
        
        dendritePosOutside = Tuple.Create<Vector3, Vector3>(resultEnd1, resultEnd2);
    }

    public void CalcDendritesEndsInside(float? yPos = null){
        Vector3 posFromWithoutY = new Vector3(dendritePosOutside.Item1.x, 0f, dendritePosOutside.Item1.z);
        Vector3 posToWithoutY = new Vector3(dendritePosOutside.Item2.x, 0f, dendritePosOutside.Item2.z);

        float distBetweenNeuronsWithoutY = (posFromWithoutY - posToWithoutY).magnitude;

        float distFromWithoutY = distBetweenNeuronsWithoutY * percentFrom;
        float distToWithoutY = distBetweenNeuronsWithoutY * percentTo;

        Vector3 destinationPointFrom = (posToWithoutY - posFromWithoutY).normalized * (distFromWithoutY - distFromMid);
        Vector3 destinationPointTo = (posFromWithoutY - posToWithoutY).normalized * (distToWithoutY - distFromMid);

        if(yPos == null){
            yPos = connection.neuronFrom.transform.position.y - (connection.neuronFrom.transform.position.y - connection.neuronTo.transform.position.y)/2;
        }

        destinationPointFrom.y = (float)yPos - connection.neuronFrom.transform.position.y;
        destinationPointTo.y = (float)yPos - connection.neuronTo.transform.position.y;

        destinationPointFrom = dendritePosOutside.Item1 + destinationPointFrom;
        destinationPointTo = dendritePosOutside.Item2 + destinationPointTo;

        dendritePosInside = Tuple.Create<Vector3, Vector3>(destinationPointFrom, destinationPointTo);
    }

    public void CalculateDendritesDirections(){
        dirFromTo = (dendritePosInside.Item1 - dendritePosOutside.Item1).normalized;
        dirToFrom = (dendritePosInside.Item2 - dendritePosOutside.Item2).normalized;
    }

    public void CalculateDendrites(float? yPos = null){
        CalcDendritesEndsOutside();
        CalcDendritesEndsInside(yPos);
        CalculateDendritesDirections();
    }

    public void SetDendritesPositionOutside(Tuple<Vector3, Vector3> outsidePos = null){
        if(outsidePos == null){
            outsidePos = dendritePosOutside;
        }

        connection.DirectLineFromTo.GetComponent<LineRenderer>().SetPosition(0, outsidePos.Item1);
        connection.DirectLineFromTo.GetComponent<LineRenderer>().SetPosition(1, outsidePos.Item2);

        connection.LineFrom.GetComponent<LineRenderer>().SetPosition(0, outsidePos.Item1);
        connection.LineTo.GetComponent<LineRenderer>().SetPosition(0, outsidePos.Item2);
    }

    public void SetDendritesPositionInside(Tuple<Vector3, Vector3> insidePos = null){
        if(insidePos == null){
            insidePos = dendritePosInside;
        }

        connection.LineFrom.GetComponent<LineRenderer>().SetPosition(1, insidePos.Item1);
        connection.LineTo.GetComponent<LineRenderer>().SetPosition(1, insidePos.Item2);

        UpdateSynapsesPosition();
        UpdateCollidersPosition();
    }

    public void UpdateSynapsesPosition(){
        connection.synapseImgFrom.transform.position = connection.LineFrom.GetComponent<LineRenderer>().GetPosition(1) + CountSynapseShift(dirFromTo, connection.synapseImageSize/2f);
        connection.synapseImgTo.transform.position = connection.LineTo.GetComponent<LineRenderer>().GetPosition(1) + CountSynapseShift(dirToFrom, connection.synapseImageSize/2f);
    }

    public void UpdateCollidersPosition(){
        connection.colliderFrom.center = connection.synapseImgFrom.transform.position;
        connection.colliderTo.center = connection.synapseImgTo.transform.position;
    }

    public void SetDendritesPosition(Tuple<Vector3, Vector3> outsidePos = null, Tuple<Vector3, Vector3> insidePos = null){
        SetDendritesPositionOutside(outsidePos);
        SetDendritesPositionInside(insidePos);
    }

    public void SetConnectionsAreInProgress(){
        connectionInProgress = true;
        connection.neuronFrom.GetComponent<Neuron>().UpdateConnectionInProgress();
        connection.neuronTo.GetComponent<Neuron>().UpdateConnectionInProgress();
    }

    public void SetConnectionsAreNotInProgress(){
        connectionInProgress = false;
        connection.neuronFrom.GetComponent<Neuron>().UpdateConnectionInProgress();
        connection.neuronTo.GetComponent<Neuron>().UpdateConnectionInProgress();
    }

    // 'isConnectionInitiated' should be set 'true' if function is used for the connection for the first time
    // 'isConnectionInitiated' should be set 'false' if function is used for connection update
    public void MergeIfTwoSided(bool isConnectionInitiated){
        ConnectionCreator connectionCreator = FindObjectOfType<ConnectionCreator>();
        if(connectionCreator.IsConnectionSaved(connection.neuronTo, connection.neuronFrom)){
            connectionCreator.MergeConnections(connectionCreator.FindConnection(connection.neuronTo, connection.neuronFrom).GetComponent<Connection>(), 
                                                                                gameObject.GetComponent<Connection>(), isConnectionInitiated);
        }
    }

    public void SplitIfTwoSided(){
        ConnectionCreator connectionCreator = FindObjectOfType<ConnectionCreator>();
        if(connectionCreator.IsConnectionSaved(connection.neuronTo, connection.neuronFrom)){
            // Debug.Log("split Connection between " + connection.neuronTo + " and " + connection.neuronFrom + " is saved");


            // if(connectionCreator.FindConnection(connection.neuronTo, connection.neuronFrom).GetComponent<Connection>() == null){
            //     Debug.Log("split cant find connection at " + gameObject.name + " between " + connection.neuronTo + " and " + connection.neuronFrom);
            // }
            connectionCreator.SplitConnections(connectionCreator.FindConnection(connection.neuronTo, connection.neuronFrom).GetComponent<Connection>(), 
                                                gameObject.GetComponent<Connection>());
        }
    }

    public void UpdateSimulationSpeed(){
        actualSimulationSpeed = simulationTimePanel.GetComponent<TimeManagement>().GetActualSimulationSpeed();
    }
}
