using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathHelp;

public class MoveNeuron : MonoBehaviour
{
    public float scrollSpeed = 0.4f; 

    public float rightLeftLimit = 20.0f;
    public float topBottomLimit = 20.0f;

    public float downLimit = -20;
    public float upLimit = 0;

    Vector3 lastMousePos;
    public bool mouseLeftButtDown = false;

    Neuron neuron;
    NeuronTextManager neuronTextManager;
    //MoveCamera moveCamera;

    private void Awake() {
        neuron = gameObject.GetComponent<Neuron>();
        neuronTextManager = gameObject.GetComponent<NeuronTextManager>();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){
            mouseLeftButtDown = true;
        }
    }

    private void Update() {
        if (Input.GetMouseButtonUp(0)){
            mouseLeftButtDown = false;
        }
        
        if(mouseLeftButtDown){
            if(!neuron.connectionInProgress && !neuron.signalInMove){
                Plane plane = new Plane(Vector3.up, -gameObject.transform.position.y); // moving neuron left/right/top/bottom with mouse move

                float distance;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    Vector3 worldPosition = ray.GetPoint(distance);

                    SetNeuronPosition(worldPosition);
                }
            }
        }
    }

    public void lowerTheNeuron(){
        Vector3 newPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - scrollSpeed, gameObject.transform.position.z);
        SetNeuronPosition(newPos);
    }

    public void upperTheNeuron(){
        Vector3 newPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + scrollSpeed, gameObject.transform.position.z);
        SetNeuronPosition(newPos);
    }

    public void SetNeuronPosition(Vector3 neuronPosition){
        if(neuronPosition.x < -rightLeftLimit) // right/left bound
            neuronPosition = new Vector3(-rightLeftLimit, neuronPosition.y, neuronPosition.z);  
        else if(neuronPosition.x > rightLeftLimit)
            neuronPosition = new Vector3(rightLeftLimit, neuronPosition.y, neuronPosition.z);  

        if(neuronPosition.z < -topBottomLimit) // top/bottom bound
            neuronPosition = new Vector3(neuronPosition.x, neuronPosition.y, -topBottomLimit);  
        else if(neuronPosition.z > topBottomLimit)
            neuronPosition = new Vector3(neuronPosition.x, neuronPosition.y, topBottomLimit);

        if(neuronPosition.y < downLimit) // up/down bound
            neuronPosition = new Vector3(neuronPosition.x, downLimit, neuronPosition.z);  
        else if(neuronPosition.y > upLimit)
            neuronPosition = new Vector3(neuronPosition.x, upLimit, neuronPosition.z);

        gameObject.transform.position = neuronPosition;

        for(int i = 0; i < neuron.connectedNeurons.Count; i++){ // update connections positions
            ConnectionStretching connectionStretching = neuron.connections[i].GetComponent<ConnectionStretching>();

            connectionStretching.CalculateDendrites();
            connectionStretching.SetDendritesPosition(connectionStretching.dendritePosOutside, connectionStretching.dendritePosInside);

            connectionStretching.MergeIfTwoSided(false);
        }

        neuronTextManager.UpdateTextPositionTowardsCamera();
    }
}
