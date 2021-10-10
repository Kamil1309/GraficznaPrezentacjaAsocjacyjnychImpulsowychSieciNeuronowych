using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynapsesVisability : MonoBehaviour
{
    public bool buttonPressed = true;

    public Sprite onImg;
    public Sprite offImg;

    public Button visibilityButton;

    public GameObject Connections;
    List<Transform> allConnections;

    private void Start() {
        visibilityButton.GetComponent<Image>().sprite = onImg;
    }

    public void VisibilityButtonPushed(){
        buttonPressed = !buttonPressed;

        if(buttonPressed){
            visibilityButton.GetComponent<Image>().sprite = onImg;
            ShowSynapses();
        }
            
        else{
            visibilityButton.GetComponent<Image>().sprite = offImg;
            HideSynapses();
        }
    }

    public void ShowSynapses(){
        allConnections = Connections.transform.GetAllChildren();

        foreach(Transform connection in allConnections){
            connection.gameObject.GetComponent<Connection>().ShowSynapses();
        }
    }

    public void HideSynapses(){
        allConnections = Connections.transform.GetAllChildren();

        foreach(Transform connection in allConnections){
            connection.gameObject.GetComponent<Connection>().HideSynapses();
        }
    }
}
