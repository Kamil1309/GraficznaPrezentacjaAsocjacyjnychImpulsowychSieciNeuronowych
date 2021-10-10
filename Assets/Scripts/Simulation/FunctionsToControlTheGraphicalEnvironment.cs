using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionsToControlTheGraphicalEnvironment : MonoBehaviour
{
    public GameObject neurons;
    public GameObject neuronPrefab;

    public GameObject connections;


    // private void Update() {
    //     if (Input.GetKeyDown("space"))
    //     {
    //         CreateNeuron("Abcd");
    //         CreateNeuron("Cdab");
    //         CreateConnection("Abcd", "Cdab");
    //     }
    // }


    public void CreateNeuron(string neuronName){
        Vector3 newPlace = neurons.GetComponent<FreePlaceFinder>().FindFreePlace();

        GameObject addedNeuron = Instantiate(neuronPrefab, newPlace, Quaternion.identity);
        addedNeuron.transform.parent = neurons.transform; 

        addedNeuron.name = neuronName;
        addedNeuron.GetComponent<Neuron>().SetNeuronName(neuronName);
    }

    
    //Function Returns list of neurons names, index at list is also it is also the index through which we can point to neurons in other function
    public List<string> GetListOfNeurons(){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        List<string> neuronsNames = new List<string>();

        for(int i = 0; i < allNeurons.Count; i++){
            neuronsNames.Add(allNeurons[i].name);
        }
        
        return neuronsNames;
    }

    public void DestroyNeuron(string neuronName){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == neuronName){
                DestroyNeuron(i);
                break;
            }
        }
    }

    public void DestroyNeuron(int neuronIndex){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        Neuron neuron =  allNeurons[neuronIndex].GetComponent<Neuron>();
        ConnectionCreator connectionCreator = connections.GetComponent<ConnectionCreator>();

        List<GameObject> connectionsToDestroy = new List<GameObject>();
        for(int i = 0; i < neuron.connections.Count; i++){
            connectionsToDestroy.Add(neuron.connections[i]);
            if(neuron.connectionsDir[i] == "from"){
                connectionCreator.DeleteConnection(neuron.gameObject, neuron.connectedNeurons[i]);
            }else if(neuron.connectionsDir[i] == "to"){
                connectionCreator.DeleteConnection(neuron.connectedNeurons[i], neuron.gameObject);
            }
        }
        IEnumerator waitForDestroy = WaitForConnectionsDestroy(connectionsToDestroy, neuron.gameObject);
        StartCoroutine(waitForDestroy);
    }

    private IEnumerator WaitForConnectionsDestroy(List<GameObject> connectionsToDestroy, GameObject neuronToDestroy){
        bool allConnectionsDestroyed = false;

        while(!allConnectionsDestroyed){
            allConnectionsDestroyed = true;
            Debug.Log("Still Exists 01");
            for(int i = 0; i < connectionsToDestroy.Count; i++){
                Debug.Log("Still Exists 02");
                if(connectionsToDestroy[i]){
                    Debug.Log("Still Exists 03");
                    allConnectionsDestroyed = false;
                    yield return new WaitForSeconds(0.1f);
                    break;
                }
            }
        }
        Destroy(neuronToDestroy);
    }

    public void CreateConnection(string neuronNameFrom, string neuronNameTo){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        int indexFrom = -1; 
        int indexTo = -1;

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == neuronNameFrom){
                indexFrom = i;
            }
            if(allNeurons[i].name == neuronNameTo){
                indexTo = i;
            }
            if(indexFrom != -1 & indexTo != -1){
                CreateConnection(indexFrom, indexTo);
                break;
            }
        }
    }

    public void CreateConnection(int neuronIndexFrom, int neuronIndexTo){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        connections.GetComponent<ConnectionCreator>().CreateConnection( allNeurons[neuronIndexFrom].gameObject, allNeurons[neuronIndexTo].gameObject );
    }

    public void DestroyConnection(string neuronNameFrom, string neuronNameTo){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        int indexFrom = -1; 
        int indexTo = -1;

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == neuronNameFrom){
                indexFrom = i;
            }
            if(allNeurons[i].name == neuronNameTo){
                indexTo = i;
            }
            if(indexFrom != -1 & indexTo != -1){
                DestroyConnection(indexFrom, indexTo);
                break;
            }
        }
    }

    public void DestroyConnection(int neuronFromIndex, int neuronToIndex){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        ConnectionCreator connectionCreator = connections.GetComponent<ConnectionCreator>();
        if(connectionCreator.IsConnectionSaved(allNeurons[neuronFromIndex].gameObject, allNeurons[neuronToIndex].gameObject)){
            connectionCreator.DeleteConnection(allNeurons[neuronFromIndex].gameObject, allNeurons[neuronToIndex].gameObject);
        }
    }

    public void SetNeuronName(string nameOfNeuronToChange, string newName){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == nameOfNeuronToChange){
                SetNeuronName(i, newName);
                break;
            }
        }
    }

    public void SetNeuronName(int neuronIndex, string newName){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        allNeurons[neuronIndex].gameObject.name = newName;
        allNeurons[neuronIndex].gameObject.GetComponent<Neuron>().SetNeuronName(newName);
    }

    //possible states: Activation, Loading, Refraction
    public void SetNeuronState(string neuronName, string state){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        int neuronIndex = -1; 

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == neuronName){
                neuronIndex = i;
            }
        }
        if(neuronIndex > -1){
            SetNeuronState(neuronIndex, state);
        }else{
            Debug.Log("Neuron not found");
        }
    }

    public void SetNeuronState(int neuronIndex, string state){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        Neuron neuron = allNeurons[neuronIndex].gameObject.GetComponent<Neuron>();
        if(state == "Activation"){
            neuron.SetActivationState();
        }else if(state == "Loading"){
            neuron.SetLoadingState();
        }else if(state == "Refraction"){
            neuron.SetRefractionState();
        }
    }

    public void SetNeuronParameters(string neuronName, float TotRelaxRate = 0, float AfterActivationRelaxRate = 0, float ActivTreshold = 0, float ExcitedState = 0){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        int neuronIndex = -1; 

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == neuronName){
                neuronIndex = i;
            }
        }
        if(neuronIndex > -1){
            SetNeuronParameters(neuronIndex, TotRelaxRate, AfterActivationRelaxRate, ActivTreshold, ExcitedState);
        }else{
            Debug.Log("Neuron not found");
        }
    }

    public void SetNeuronParameters(int neuronIndex, float TotRelaxRate = 0, float AfterActivationRelaxRate = 0, float ActivTreshold = 0, float ExcitedState = 0){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        Neuron neuron = allNeurons[neuronIndex].gameObject.GetComponent<Neuron>();
        neuron.SetParameters(TotRelaxRate, AfterActivationRelaxRate, ActivTreshold, ExcitedState);
    }

    public void SetConnectionValue(string neuronFromName, string neuronToName, float newConnectionValue){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        int indexFrom = -1; 
        int indexTo = -1;

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == neuronFromName){
                indexFrom = i;
            }
            if(allNeurons[i].name == neuronToName){
                indexTo = i;
            }
            if(indexFrom != -1 & indexTo != -1){
                SetConnectionValue(indexFrom, indexTo, newConnectionValue);
                break;
            }
        }
    }

    public void SetConnectionValue(int neuronFromIndex, int NeuronToIndex, float newConnectionValue){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        ConnectionCreator connectionCreator = connections.GetComponent<ConnectionCreator>();

        GameObject neuronFrom = allNeurons[neuronFromIndex].gameObject;
        GameObject neuronTo = allNeurons[NeuronToIndex].gameObject;

        GameObject foundConnection = connectionCreator.FindConnection(neuronFrom, neuronTo);
    
        foundConnection.GetComponent<Connection>().SetSynapsValue(newConnectionValue);
    }

    public void SendSignal(string neuronFromName, string neuronToName){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        int indexFrom = -1; 
        int indexTo = -1;

        for(int i = 0; i < allNeurons.Count; i++){
            if(allNeurons[i].name == neuronFromName){
                indexFrom = i;
            }
            if(allNeurons[i].name == neuronToName){
                indexTo = i;
            }
            if(indexFrom != -1 & indexTo != -1){
                SendSignal(indexFrom, indexTo);
                break;
            }
        }
    }

    public void SendSignal(int neuronFromIndex, int neuronToIndex){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        allNeurons[neuronFromIndex].GetComponent<Neuron>().SendSignal(allNeurons[neuronToIndex].gameObject);
    }
}
