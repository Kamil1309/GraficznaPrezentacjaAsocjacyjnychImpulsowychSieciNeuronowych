using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanelsText : MonoBehaviour
{
    Neuron neuron;

    private void Awake() {
        neuron = GetComponent<Neuron>();
    }

    public void SetAllPanelsTexts(){
        SetTopPanelText();
        SetBottomPanelText();
        SetRightPanelText();
        SetLeftPanelText();
    }

    void SetTopPanelText(){
        neuron.panelTop.GetComponent<PanelProperties>().panelsText.text = neuron.neuronParams[0].ToString();
    }

    void SetBottomPanelText(){
        neuron.panelBot.GetComponent<PanelProperties>().panelsText.text = neuron.neuronParams[1].ToString();
    }

    void SetRightPanelText(){
        neuron.panelRight.GetComponent<PanelProperties>().panelsText.text = neuron.neuronParams[2].ToString();
    }

    void SetLeftPanelText(){
        neuron.panelLeft.GetComponent<PanelProperties>().panelsText.text = neuron.neuronParams[3].ToString();
    }
}
