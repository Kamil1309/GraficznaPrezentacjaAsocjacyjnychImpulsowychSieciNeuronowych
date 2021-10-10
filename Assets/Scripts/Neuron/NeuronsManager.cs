using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronsManager : MonoBehaviour
{
    [SerializeField] GameObject synapses;

    public void SwitchPropertiesColors(bool colorIsWhiteOrg){
        List<Transform> allNeurons = gameObject.transform.GetAllChildren();
        foreach(Transform neuron in allNeurons){
            neuron.gameObject.GetComponent<ChangeLinesColor>().SwitchColor(colorIsWhiteOrg);
        }
    }

    private void Start() {
        synapses.GetComponent<ConnectionInitializator>().InitializeConnections();
    }

    public void UpdateAllNeuronsTextPosition(){
        List<Transform> allNeurons = gameObject.transform.GetAllChildren();
        foreach(Transform neuron in allNeurons){
            neuron.gameObject.GetComponent<NeuronTextManager>().UpdateTextPositionTowardsCamera();
        }
    }
}
