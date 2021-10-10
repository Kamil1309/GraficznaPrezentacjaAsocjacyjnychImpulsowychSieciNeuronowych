using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using MathHelp;
using System.Collections.Generic;

public class Connection : Selectable
{
    [SerializeField] public RawImage synapseImgFrom;
    [SerializeField] public RawImage synapseImgTo;
    [SerializeField] public Text synapseText;

    [SerializeField] public GameObject LineFrom;
    [SerializeField] public GameObject LineTo;
    [SerializeField] public GameObject DirectLineFromTo;
    
    [SerializeField] public SphereCollider colliderFrom;
    [SerializeField] public SphereCollider colliderTo;
    
    ConnectionStretching connectionStretching;
    
    public GameObject neuronFrom;
    public GameObject neuronTo;

    [HideInInspector] public float synapseImageSize = 0.8f;
    [HideInInspector] public int synapseTextSize = 50;

    [SerializeField] float maxSynapseImgSize = 2f;
    [SerializeField] float minSynapseImgSize = 0.3f;

    [SerializeField] Text synapsText;
    public float synapsValue;

    public bool isConnectionMerged = false;

    public override bool CanBeSelected()
    {
        return true;
    }

    public override void Select()
    {
        //Debug.Log("You have selected " + gameObject.name);
    }

    public override void OnMouseWheelMovingPlus(){
        if(!connectionStretching.connectionInProgress){
            if(synapseImageSize + 0.05 < maxSynapseImgSize){
                SetSynapsesSize(synapseImageSize + 0.05f);
            }
        }
    }

    public override void OnMouseWheelMovingMinus(){
        if(!connectionStretching.connectionInProgress){
            if(synapseImageSize - 0.05 > minSynapseImgSize){
                SetSynapsesSize(synapseImageSize - 0.05f);
            }
        }
    }

    public override string CreateTitleText(){
        string title = "Connection between Neuron '" + neuronFrom.GetComponent<Neuron>().neuronText.text + "' and Neuron '" + neuronTo.GetComponent<Neuron>().neuronText.text + "'";
        return title;
    }

    public override string CreateDescriptionText(){
        string description = "Connection value : " + synapsValue.ToString("F1") + "\n";

        return description;
    }

    public void InitConnection(GameObject neuronF, GameObject neuronT){
        if(FindObjectOfType<SynapsesVisability>().buttonPressed){
            ShowSynapses();
        }else{
            HideSynapses();
        }

        neuronFrom = neuronF;
        neuronTo = neuronT;
        
        connectionStretching = GetComponent<ConnectionStretching>();
        connectionStretching.InitStreatchingData();
        
        if(FindObjectOfType<ChangeConnectionsColor>().colorIsWhite){
            SetDendritesColor(new Color(0.0f, 0.0f, 0.0f, 1f));
        }else{
            SetDendritesColor(new Color(1f, 1f, 1f, 1f));
        }
    }

    public void ShowSynapses(){
        synapseImgFrom.gameObject.SetActive(true);
        synapseImgTo.gameObject.SetActive(true);

        LineFrom.SetActive(true);
        LineTo.SetActive(true);
        DirectLineFromTo.SetActive(false);

        colliderFrom.enabled = true;
        colliderTo.enabled = true;
    }

    public void HideSynapses(){
        synapseImgFrom.gameObject.SetActive(false);
        synapseImgTo.gameObject.SetActive(false);

        LineFrom.SetActive(false);
        LineTo.SetActive(false);
        DirectLineFromTo.SetActive(true);

        colliderFrom.enabled = false;
        colliderTo.enabled = false;
    }

    public void SetDendritesColor(Color newColor){
        LineFrom.GetComponent<LineRenderer>().startColor = newColor;
        LineFrom.GetComponent<LineRenderer>().endColor = newColor;

        LineTo.GetComponent<LineRenderer>().startColor = newColor;
        LineTo.GetComponent<LineRenderer>().endColor = newColor;

        DirectLineFromTo.GetComponent<LineRenderer>().startColor = newColor;
        DirectLineFromTo.GetComponent<LineRenderer>().endColor = newColor;
    }

    public void SetSynapsesSize(float size){
        synapseImageSize = size;
        
        connectionStretching.distFromMid = 5/4f * synapseImageSize/2f;

        synapseImgFrom.rectTransform.sizeDelta = new Vector2(size, size);
        synapseImgTo.rectTransform.sizeDelta = new Vector2(size, size);

        colliderFrom.radius = synapseImageSize/2f;
        colliderTo.radius = synapseImageSize/2f;

        connectionStretching.CalcDendritesEndsInside();
        connectionStretching.SetDendritesPositionInside();

        SetSynapsTextSize((int)(size * 50/0.8f)); 
    }

    public void SetSynapsTextSize(int textSize){
        synapseTextSize = textSize;

        synapseText.fontSize = textSize;
    }

    public void SetSynapsValue(float value){
        if(value > 100f){
            value = 100f;
            Debug.LogWarning("You tried to set synapse value over 100 so it has been changed to 100");
        }
        if(value < 0f){
            value = 0f;
            Debug.LogWarning("You tried to set synapse value below 0 so it has been changed to 0");
        }

        float stringValue = value;// Set text in synapse 

        if(MathHelp.CheckBounds.IsBetween(stringValue, 99.49f, 100f)){
            stringValue = 100;
            synapsText.text = stringValue.ToString();
        }
        if(MathHelp.CheckBounds.IsBetween(stringValue, 0.99f, 99.49f)){
            stringValue = Mathf.Round(stringValue);
            synapsText.text = stringValue.ToString();
        }
        if(MathHelp.CheckBounds.IsBetween(stringValue, 0f, 0.99f)){
            stringValue = Mathf.Round(stringValue * 10);
            synapsText.text = "." + stringValue.ToString();
        }

        if(MathHelp.CheckBounds.IsBetween(stringValue, 0f, 20f)){
            SetSynapsesSize(0.4f);
        }
        if(MathHelp.CheckBounds.IsBetween(stringValue, 20f, 40f)){
            SetSynapsesSize(0.5f);
        }
        if(MathHelp.CheckBounds.IsBetween(stringValue, 40f, 60f)){
            SetSynapsesSize(0.6f);
        }
        if(MathHelp.CheckBounds.IsBetween(stringValue, 60f, 80f)){
            SetSynapsesSize(0.7f);    
        }
        if(MathHelp.CheckBounds.IsBetween(stringValue, 80f, 100f)){
            SetSynapsesSize(0.8f);
        }

        SetSynapseColor(new Color( (100f - value)/100f, 1f, (100f - value)/100f, 1f) );

        synapsValue = value;
    }

    public void SetSynapseColor(Color newColor){
        synapseImgTo.color = newColor;
    }
}
