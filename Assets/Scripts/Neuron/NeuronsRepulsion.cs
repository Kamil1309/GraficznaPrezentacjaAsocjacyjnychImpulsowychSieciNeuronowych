using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronsRepulsion : MonoBehaviour
{
    Vector3 shiftVector = new Vector3(0.0f, 0.0f, 0.0f);
    
    [SerializeField] float repulsiveForce;
    [SerializeField] float attractiveForce;
    [SerializeField] float repulsiveForceTrashold;
    [SerializeField] float attractiveForceTrashold;

    Neuron neuron; 
    MoveNeuron moveNeuron;

    [HideInInspector] public float actualSimulationSpeed = 1.0f;

    private void Start() {
        neuron = GetComponent<Neuron>();
        moveNeuron = GetComponent<MoveNeuron>();

        InvokeRepeating("PushTheNeurons", 0.1f, 0.1f);
    }

    public void setActualSimulationSpeed(float newSpeed){
        actualSimulationSpeed = newSpeed;
    }

    private void PushTheNeurons(){
        List<Transform> allNeurons = gameObject.transform.parent.GetAllChildren();
        
        float distBetweenNeurons;
        Vector3 forceDirection;
        float deltaForce;

        shiftVector = Vector3.zero;

        bool cantMove = false;
        
        // Repulsive force
        for(int neuronNum = 0; neuronNum < allNeurons.Count; neuronNum++){
            if(!ReferenceEquals(gameObject, allNeurons[neuronNum].gameObject)){
                forceDirection = Vector3.Normalize(gameObject.transform.position - allNeurons[neuronNum].transform.position);
                distBetweenNeurons = (gameObject.transform.position - allNeurons[neuronNum].transform.position).magnitude;

                deltaForce = repulsiveForce/(distBetweenNeurons * distBetweenNeurons)* actualSimulationSpeed;  
                if(deltaForce > repulsiveForceTrashold * actualSimulationSpeed){
                    shiftVector += forceDirection * deltaForce ;
                }
            }
        }
        // Attraction force
        for(int connNum = 0; connNum < neuron.connections.Count; connNum++){
            if(neuron.connectionInProgress || neuron.signalInMove){
                cantMove = true;
                break;
            }
            
            if(neuron.connectionsDir[connNum] == "from"){
                forceDirection = Vector3.Normalize(neuron.connections[connNum].GetComponent<Connection>().neuronTo.transform.position - gameObject.transform.position);
                distBetweenNeurons = (neuron.connections[connNum].GetComponent<Connection>().neuronTo.transform.position - gameObject.transform.position).magnitude;
            }else{
                forceDirection = Vector3.Normalize(neuron.connections[connNum].GetComponent<Connection>().neuronFrom.transform.position - gameObject.transform.position);
                distBetweenNeurons = (neuron.connections[connNum].GetComponent<Connection>().neuronFrom.transform.position - gameObject.transform.position).magnitude;
            }

            deltaForce = neuron.connections[connNum].GetComponent<Connection>().synapsValue * attractiveForce * attractiveForce*(distBetweenNeurons * distBetweenNeurons) * actualSimulationSpeed;
               
            if(deltaForce > attractiveForceTrashold * actualSimulationSpeed){
                shiftVector += forceDirection * deltaForce;
                shiftVector.y = 0;
            }
        }
        if(!cantMove){
            moveNeuron.SetNeuronPosition(gameObject.transform.position + shiftVector);
        }
    }
}
