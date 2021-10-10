using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLinesColor : MonoBehaviour
{
    public void SwitchColor(bool colorIsWhite){
        GameObject[] lines = gameObject.GetComponent<Neuron>().lines;
        
        Color newColor;
        if(colorIsWhite){
            newColor = new Color(0.0f, 0.0f, 0.0f, 1f);
        }
        else{
            newColor = new Color(1f, 1f, 1f, 1f);
        }

        foreach(GameObject line in lines){
            line.GetComponent<LineRenderer>().startColor = newColor;
            line.GetComponent<LineRenderer>().endColor = newColor;
        }
    }
}