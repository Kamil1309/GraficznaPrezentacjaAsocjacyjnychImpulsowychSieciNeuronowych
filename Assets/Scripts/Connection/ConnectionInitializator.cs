using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionInitializator : MonoBehaviour
{
    public GameObject connectionPrefab; 

    public GameObject neurons;

    /// <summary>
    ///Checks if 'connected neuron' is assigned on both sides (on both neurons), if not it adds connected neuron and connection direction to proper lists
    ///</summary>
    public void InitializeConnections(){
        List<Transform> allNeurons = neurons.transform.GetAllChildren();

        for(int neuronNum = 0; neuronNum < allNeurons.Count; neuronNum++){
            Neuron actualNeuronProp = allNeurons[neuronNum].gameObject.GetComponent<Neuron>();
            List<GameObject> actualConnectedNeurons = actualNeuronProp.connectedNeurons;
        
            CheckPropertiesCorrectnessInit(actualNeuronProp);

            for(int connectedNeuronNum = 0; connectedNeuronNum < actualConnectedNeurons.Count; connectedNeuronNum++){
                Neuron connectedNeuronProp = actualConnectedNeurons[connectedNeuronNum].gameObject.GetComponent<Neuron>();

                if(actualNeuronProp.IsConnectedWith2(connectedNeuronProp.gameObject) == 2){
                    var indexes = actualNeuronProp.FindConnectedNeuronIndex(connectedNeuronProp.gameObject);

                    actualNeuronProp.DeleteConnectedNeuron((int)indexes.Item2);
                    actualNeuronProp.DeleteConnectedNeuron((int)indexes.Item1);

                    actualNeuronProp.DeleteConnectionDir((int)indexes.Item2);
                    actualNeuronProp.DeleteConnectionDir((int)indexes.Item1);

                    indexes = connectedNeuronProp.FindConnectedNeuronIndex(actualNeuronProp.gameObject);

                    if(indexes.Item2 != null){
                        connectedNeuronProp.DeleteConnectedNeuron((int)indexes.Item2);
                        connectedNeuronProp.DeleteConnectionDir((int)indexes.Item2);
                    }else if(indexes.Item1 != null){
                        connectedNeuronProp.DeleteConnectedNeuron((int)indexes.Item1);
                        connectedNeuronProp.DeleteConnectionDir((int)indexes.Item1);
                    }

                    int index1 = actualNeuronProp.AddConnectedNeuron(connectedNeuronProp.gameObject);
                    actualNeuronProp.AddConnectionDir(index1, "from");
                    index1 = actualNeuronProp.AddConnectedNeuron(connectedNeuronProp.gameObject);
                    actualNeuronProp.AddConnectionDir(index1, "to");

                    int index2 = connectedNeuronProp.AddConnectedNeuron(actualNeuronProp.gameObject);
                    connectedNeuronProp.AddConnectionDir(index2, "to");
                    index2 = connectedNeuronProp.AddConnectedNeuron(actualNeuronProp.gameObject);
                    connectedNeuronProp.AddConnectionDir(index2, "from");

                    GetComponent<ConnectionCreator>().AddToConnectionList(actualNeuronProp.gameObject, connectedNeuronProp.gameObject);
                    GetComponent<ConnectionCreator>().AddToConnectionList(connectedNeuronProp.gameObject, actualNeuronProp.gameObject);

                    CreateConnectionWhileInit(actualNeuronProp.gameObject, connectedNeuronProp.gameObject, index1 - 1, index2 - 1);
                    CreateConnectionWhileInit(connectedNeuronProp.gameObject, actualNeuronProp.gameObject, index1, index2); 

                }else if(actualNeuronProp.IsConnectedWith2(connectedNeuronProp.gameObject) == 1){
                    var indexes = connectedNeuronProp.FindConnectedNeuronIndex(allNeurons[neuronNum].gameObject);
                    int index = (int)indexes.Item1;

                    if(actualNeuronProp.connectionsDir[neuronNum] == "from"){
                        connectedNeuronProp.AddConnectionDir(index, "to");
                        GetComponent<ConnectionCreator>().AddToConnectionList(actualNeuronProp.gameObject, connectedNeuronProp.gameObject);
                        CreateConnectionWhileInit(actualNeuronProp.gameObject, connectedNeuronProp.gameObject);
                    }else if(actualNeuronProp.connectionsDir[neuronNum] == "to"){
                        connectedNeuronProp.AddConnectionDir(index, "from");
                        GetComponent<ConnectionCreator>().AddToConnectionList(connectedNeuronProp.gameObject, actualNeuronProp.gameObject);
                        CreateConnectionWhileInit(connectedNeuronProp.gameObject, actualNeuronProp.gameObject);
                    }
                }
            }
        }  
    }

    bool CheckConnectionIsSave(Neuron neuronProp1, Neuron neuronProp2, string neuron1Dir){
        bool isConnectionSavedB = false;

        if(neuron1Dir == "from"){
            if(GetComponent<ConnectionCreator>().IsConnectionSaved(neuronProp1.gameObject, neuronProp2.gameObject)){
                if(GetComponent<ConnectionCreator>().areMultipleConnectionsPossible == false){
                    isConnectionSavedB = true;
                }
            } 
        }else if(neuron1Dir == "to"){
            if(GetComponent<ConnectionCreator>().IsConnectionSaved(neuronProp2.gameObject, neuronProp1.gameObject)){
                if(GetComponent<ConnectionCreator>().areMultipleConnectionsPossible == false){
                    isConnectionSavedB = true;
                }
            } 
        }

        return isConnectionSavedB;
    }

    void CheckPropertiesCorrectnessInit(Neuron actualNeuronProp){ // Checks if properties added by user (by hand in Unity) are correct
        if(actualNeuronProp.connectionsDir.Count !=  actualNeuronProp.connectedNeurons.Count){  // Error report, neuron got connected neuron assigned but dont have direction
            Debug.LogError("The number of connected neurons and connection directions is not equal : '" + actualNeuronProp.gameObject.name + "'  ,please fill in the missing place");
        }else{
            for(int connectedNeuronNum = 0; connectedNeuronNum < actualNeuronProp.connectedNeurons.Count; connectedNeuronNum++){
                if(actualNeuronProp.connectionsDir[connectedNeuronNum] != "from" && actualNeuronProp.connectionsDir[connectedNeuronNum] != "to"){
                    Debug.LogError("Wrong dendrite direction name at neuron: '" + actualNeuronProp.gameObject.name + "' and at position on list: " + connectedNeuronNum);
                }
            }
        }
    }

    void CreateConnectionWhileInit(GameObject neuronFrom, GameObject neuronTo, int indexFrom = -1, int indexTo = -1){
        Neuron neuronFromProp = neuronFrom.GetComponent<Neuron>();
        Neuron neuronToProp = neuronTo.GetComponent<Neuron>();

        GameObject addedDendrite = Instantiate(connectionPrefab, new Vector3(0,0,0), Quaternion.identity);
        addedDendrite.transform.parent = gameObject.transform;

        if(indexFrom == -1){
            indexFrom = neuronFromProp.connectedNeurons.FindIndex(obj => GameObject.ReferenceEquals( obj, neuronTo));
        }
        if(indexTo == -1){
            indexTo = neuronToProp.connectedNeurons.FindIndex(obj => GameObject.ReferenceEquals( obj, neuronFrom));
        }
        neuronFromProp.AddConnection(indexFrom, addedDendrite);
        neuronToProp.AddConnection(indexTo, addedDendrite);

        addedDendrite.GetComponent<Connection>().InitConnection(neuronFrom, neuronTo);
        addedDendrite.GetComponent<ConnectionStretching>().ExpandConnection();
    }
}


