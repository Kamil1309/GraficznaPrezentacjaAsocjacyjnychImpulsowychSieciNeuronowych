using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionSignal : MonoBehaviour
{
    [SerializeField] GameObject signalLine;
    private LineRenderer signalLineRend;
    
    [SerializeField] float signalVelocity;
    [SerializeField] float signalLength;

    private IEnumerator signalCourtine;
    private IEnumerator forwardCourtine;

    public bool signalInMovePartly = false; // variable used for parts of route that is traversed by the signal
    public bool signalInMoveFull = false; // variable used for whole route that is traversed by the signal

    [HideInInspector] public float actualSimulationSpeed = 1.0f;
    [HideInInspector] public GameObject simulationTimePanel;

    private void Start() {
        signalLineRend = signalLine.GetComponent<LineRenderer>();

        UpdateSimulationSpeed();
    }

    //Function that starts the courtine for the animation of the transmitted signal (to use from outside)
    public void SendSignalForward(){
        SetSignalIsInMove(); // Set flag as true
        Connection connection = GetComponent<Connection>();

        if(connection.isConnectionMerged){
            GameObject connectionObjBackward = FindObjectOfType<ConnectionCreator>().FindConnection(connection.neuronTo, connection.neuronFrom);
        
            forwardCourtine = SendSignalForwardCourtine(connection, connectionObjBackward);
        }else{
            forwardCourtine = SendSignalForwardCourtine(connection);
        }

        StartCoroutine(forwardCourtine);
    }

    //Courtine responsible for the animation of the transmitted signal from neuron to neuron
    private IEnumerator SendSignalForwardCourtine(Connection connection, GameObject connectionObjBackward = null){

        if(FindObjectOfType<SynapsesVisability>().buttonPressed){
            if(connectionObjBackward != null){
                Connection connectionBackward = connectionObjBackward.GetComponent<Connection>();

                Vector3 startPosBackward = connectionBackward.LineTo.GetComponent<LineRenderer>().GetPosition(0);
                Vector3 endPosBackward = connectionBackward.LineTo.GetComponent<LineRenderer>().GetPosition(1);

                signalCourtine = connectionBackward.GetComponent<ConnectionSignal>().SendSignalThrough(startPosBackward, endPosBackward);
                StartCoroutine(signalCourtine);

                while(connectionBackward.GetComponent<ConnectionSignal>().signalInMovePartly){
                    yield return new WaitForSeconds(0.1f);
                }
                //connectionBackward.GetComponent<ConnectionSignal>().signalInMove = true;
            }

            Vector3 startPos = connection.LineFrom.GetComponent<LineRenderer>().GetPosition(0);
            Vector3 midPos1 = connection.LineFrom.GetComponent<LineRenderer>().GetPosition(1);

            Vector3 midPos2 = connection.LineTo.GetComponent<LineRenderer>().GetPosition(1);
            Vector3 endPos = connection.LineTo.GetComponent<LineRenderer>().GetPosition(0);
            
            signalCourtine = SendSignalThrough(startPos, midPos1);
            StartCoroutine(signalCourtine);

            while(signalInMovePartly){
                yield return new WaitForSeconds(0.1f);
            }

            signalCourtine = SendSignalThrough(midPos2, endPos);
            StartCoroutine(signalCourtine);

            while(signalInMovePartly){
                yield return new WaitForSeconds(0.1f);
            }
        }else{
            Vector3 startPos = connection.DirectLineFromTo.GetComponent<LineRenderer>().GetPosition(0);
            Vector3 endPos = connection.DirectLineFromTo.GetComponent<LineRenderer>().GetPosition(1);

            signalCourtine = SendSignalThrough(startPos, endPos);
            StartCoroutine(signalCourtine);          

            while(signalInMovePartly){
                yield return new WaitForSeconds(0.1f);
            }
        }
        SetSignalIsNotInMove();

        yield break;
    }

    // "inside" courtine used by 'SendSignalForwardCourtine' responsible for sending signal between two points. 
    //Sending a signal from a neuron to a neuron requires several transmissions between two points
    private IEnumerator SendSignalThrough(Vector3 startPos, Vector3 endPos)
    {
        signalInMovePartly = true;

        bool expandPhase = true;
        bool movePhase = false;
        bool shrinkPhase = false;

        Vector3 signalDirection = Vector3.Normalize(endPos - startPos);
        Vector3 signalShift = signalDirection * signalVelocity * actualSimulationSpeed;

        SetLinePos(signalLineRend, startPos, startPos);
        signalLine.SetActive(true);

        while(true){
            if(expandPhase && Vector3.Magnitude(GetSignalStartPos() - GetSignalEndPos()) < signalLength){
                SetLinePos(signalLineRend, GetSignalStartPos() + signalShift, GetSignalEndPos());
                yield return new WaitForFixedUpdate();
            }
            if(expandPhase && Vector3.Magnitude(GetSignalStartPos() - GetSignalEndPos()) >= signalLength){
                expandPhase = false;
                movePhase = true;
                SetLinePos(signalLineRend, startPos + signalDirection * signalLength, GetSignalEndPos());
                yield return new WaitForFixedUpdate();
            }
            if(movePhase && Vector3.Magnitude(GetSignalStartPos() - startPos) < Vector3.Magnitude(endPos - startPos)){
                SetLinePos(signalLineRend, GetSignalStartPos() + signalShift, GetSignalEndPos() + signalShift);
                yield return new WaitForFixedUpdate();
            }

            if(movePhase && Vector3.Magnitude(GetSignalStartPos() - startPos) >= Vector3.Magnitude(endPos - startPos)){
                movePhase = false;
                shrinkPhase = true;
                SetLinePos(signalLineRend, endPos, GetSignalEndPos() + signalShift);
                yield return new WaitForFixedUpdate();
            }
            if(shrinkPhase && Vector3.Magnitude(GetSignalEndPos() - startPos) < Vector3.Magnitude(endPos - startPos)){
                SetLinePos(signalLineRend, endPos, GetSignalEndPos() + signalShift);
                yield return new WaitForFixedUpdate();
            }
            if(shrinkPhase && Vector3.Magnitude(GetSignalEndPos() - startPos) > Vector3.Magnitude(endPos - startPos)){
                shrinkPhase = false;
                signalLine.SetActive(false);
                signalInMovePartly = false;
                yield break;
            }
        }
    }

    // Start of signal is end that first comes to destination point
    private Vector3 GetSignalStartPos(){
        return signalLineRend.GetPosition(0);
    }

    // End of signal is end that last comes to destination point
    private Vector3 GetSignalEndPos(){
        return signalLineRend.GetPosition(1);
    }

    void SetLinePos(LineRenderer line, Vector3 startPos, Vector3 endPos){
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }

    public void SetSignalIsInMove(){
        Connection connection = GetComponent<Connection>();

        signalInMoveFull = true;
        if(connection.isConnectionMerged){
            GameObject connectionObjBackward = FindObjectOfType<ConnectionCreator>().FindConnection(connection.neuronTo, connection.neuronFrom);
            connectionObjBackward.GetComponent<ConnectionSignal>().signalInMoveFull = true;
        }

        connection.neuronFrom.GetComponent<Neuron>().UpdateSignalInMove();
        connection.neuronTo.GetComponent<Neuron>().UpdateSignalInMove();
    }

    public void SetSignalIsNotInMove(){
        Connection connection = GetComponent<Connection>();

        signalInMoveFull = false;
        if(connection.isConnectionMerged){
            GameObject connectionObjBackward = FindObjectOfType<ConnectionCreator>().FindConnection(connection.neuronTo, connection.neuronFrom);
            connectionObjBackward.GetComponent<ConnectionSignal>().signalInMoveFull = false;
        }

        GetComponent<Connection>().neuronFrom.GetComponent<Neuron>().UpdateSignalInMove();
        GetComponent<Connection>().neuronTo.GetComponent<Neuron>().UpdateSignalInMove();
    }

    private void UpdateSimulationSpeed(){
        actualSimulationSpeed = simulationTimePanel.GetComponent<TimeManagement>().GetActualSimulationSpeed();
    }
}
