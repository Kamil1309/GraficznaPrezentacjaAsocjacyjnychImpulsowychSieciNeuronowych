using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronTextManager : MonoBehaviour
{
    Camera mainCamera;

    Neuron neuron;
    
    private void Awake() {
        neuron = GetComponent<Neuron>();

        mainCamera = Camera.main;
    }

    Vector3 CalcNeuronCameraDir(Vector3 point){
        Vector3 direction = point - neuron.transform.position;
        direction = direction.normalized;

        return direction;
    }

    Vector3 CalcTextPosition(Vector3 point){
        Vector3 calculatedShift = CalcNeuronCameraDir(point) * neuron.radius;
        Vector3 calculatedPosition = neuron.transform.position + calculatedShift;

        return calculatedPosition;
    }

    void RotateTextToPoint(Vector3 point){
        Vector3 forwardVec = -CalcNeuronCameraDir(point);
        neuron.neuronText.transform.forward = forwardVec;

        Vector3 rotationShift = new Vector3(0f, 0f, 2f);
        while(Mathf.Abs(neuron.neuronText.transform.up.x) > 0.02f || neuron.neuronText.transform.up.z < 0f){
            neuron.neuronText.transform.Rotate( rotationShift );
        }
    }

    public void SetText(string newText){
        neuron.neuronText.text = newText;

        switch (newText.Length){
            case 1:
                SetTextSize(30);
                break;
            case 2:
                SetTextSize(20);
                break;
            case 3:
                SetTextSize(13);
                break;
            case 4:
                SetTextSize(10);
                break;
            default:
                SetTextSize(Mathf.Max(12 - newText.Length, 6));
                break;
        }
    }

    void SetTextSize(int textSize){
        neuron.neuronText.fontSize = textSize;
    }

    void SetTextPosition(Vector3 newPosition){
        neuron.neuronText.transform.position = newPosition;
    }

    void SetTextRotation(Vector3 newRotation){
        neuron.neuronText.transform.Rotate(newRotation);
    }

    void DirectTheTextTowardsPoint(Vector3 point){
        SetTextPosition( CalcTextPosition(point) );
        RotateTextToPoint( point );
    }

    public void UpdateTextPositionTowardsCamera(){
        SetTextPosition( CalcTextPosition(mainCamera.transform.position) );
        RotateTextToPoint(mainCamera.transform.position);
    }
}
