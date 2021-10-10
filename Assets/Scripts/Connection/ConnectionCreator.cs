using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionCreator : MonoBehaviour
{
    public GameObject connectionPrefab; 

    // List of connections composed of two lists of neurons. 
    // First list of neurons contains 'From' (the neurons from which the connection comes out) 
    // Second list of neurons contains 'To' (the neurons into which the connection enters).
    public List<GameObject>[] allConections = new List<GameObject>[2]{ new List<GameObject>(), new List<GameObject>() }; 

    [SerializeField] float startSynapseValue = 50f;

    public bool areMultipleConnectionsPossible = false;

    public GameObject simulationTimePanel;

    public void CreateConnection(GameObject neuronFrom, GameObject neuronTo){
        if(!GameObject.ReferenceEquals(neuronFrom, neuronTo)){
            if(!IsConnectionSaved(neuronFrom, neuronTo) || (IsConnectionSaved(neuronFrom, neuronTo) && areMultipleConnectionsPossible) ){
                AddToConnectionList(neuronFrom, neuronTo);

                Neuron neuronFromProp = neuronFrom.GetComponent<Neuron>();
                Neuron neuronToProp = neuronTo.GetComponent<Neuron>();

                CheckPropertiesCorrectness(neuronFrom, neuronTo);

                GameObject addedConnection = Instantiate(connectionPrefab, new Vector3(0,0,0), Quaternion.identity);
                addedConnection.transform.parent = gameObject.transform;

                neuronFromProp.AddConnectedNeuron(neuronTo);
                neuronFromProp.AddConnection(addedConnection);
                neuronFromProp.AddConnectionDir("from");

                neuronToProp.AddConnectedNeuron(neuronFrom);
                neuronToProp.AddConnection(addedConnection);
                neuronToProp.AddConnectionDir("to");

                addedConnection.GetComponent<ConnectionSignal>().simulationTimePanel = simulationTimePanel;
                addedConnection.GetComponent<ConnectionStretching>().simulationTimePanel = simulationTimePanel;
                addedConnection.GetComponent<ConnectionStretching>().UpdateSimulationSpeed();

                addedConnection.GetComponent<Connection>().InitConnection(neuronFrom, neuronTo);
                addedConnection.GetComponent<ConnectionStretching>().ExpandConnection();
                
                
            }else{
                //Debug.Log( $"You try to create connection between {neuronFrom.name} and {neuronTo.name} but it already exists" );
            }
        }else{
            //Debug.Log( $"You try to create connection between {neuronFrom.name} and {neuronTo.name} but it's the same neuron!" );
        }
    }

    public void DeleteConnection(GameObject neuronFrom, GameObject neuronTo){
        if(IsConnectionSaved(neuronFrom, neuronTo)){
            //Debug.Log(allConections[0].Count + " Deleting Connection between " + neuronFrom.name + "   " + neuronTo.name + "   " + IsConnectionSaved(neuronFrom, neuronTo));
            
            Neuron neuronFromProp = neuronFrom.GetComponent<Neuron>();
            Neuron neuronToProp = neuronTo.GetComponent<Neuron>();
            
            RemoveFromConnectionList(neuronFromProp.gameObject, neuronToProp.gameObject);

            //CheckPropertiesCorrectness(neuronFrom, neuronTo);

            GameObject deletedConnection = FindConnection(neuronFrom, neuronTo);

            //Debug.Log("Deleted connection is between " + deletedConnection.GetComponent<Connection>().neuronFrom.name + "    " + deletedConnection.GetComponent<Connection>().neuronTo.name);

            if(!deletedConnection.GetComponent<ConnectionStretching>().connectionInProgress){
                deletedConnection.GetComponent<ConnectionStretching>().ShrinkConnection();

                IEnumerator waitForShrink = WaitFroShrink(neuronFromProp, neuronToProp, deletedConnection);
                StartCoroutine(waitForShrink);
            }
        }else{
            Debug.Log("You try to delete connection but it doesn't exist");
        }
    }

    private IEnumerator WaitFroShrink(Neuron neuronFromProp, Neuron neuronToProp, GameObject deletedConnection)
    {
        while(!deletedConnection.GetComponent<ConnectionStretching>().isShrinkingFinished){
            yield return new WaitForSeconds(0.1f);
        }

        int? deletedIndexFrom = neuronFromProp.FindConnectionIndex(deletedConnection);
        int? deletedIndexTo = neuronToProp.FindConnectionIndex(deletedConnection);

        if(deletedIndexFrom == null || deletedIndexTo == null){
            Debug.LogError("Something went wrong! While deleting connection you try to find index of connection at neuron properties but it doesnt exists there");
        }

        neuronFromProp.DeleteConnectedNeuron((int)deletedIndexFrom);
        neuronFromProp.DeleteConnectionFromList((int)deletedIndexFrom);
        neuronFromProp.DeleteConnectionDir((int)deletedIndexFrom);

        neuronToProp.DeleteConnectedNeuron((int)deletedIndexTo);
        neuronToProp.DeleteConnectionFromList((int)deletedIndexTo);
        neuronToProp.DeleteConnectionDir((int)deletedIndexTo);

        // RemoveFromConnectionList(neuronFromProp.gameObject, neuronToProp.gameObject);

        Destroy(deletedConnection);

        yield break;
    }

    public void CheckPropertiesCorrectness(GameObject neuronFrom, GameObject neuronTo){
        List<GameObject> neurons = new List<GameObject>();
        neurons.Add(neuronFrom);
        neurons.Add(neuronTo);

        for(int neuronNum = 0; neuronNum < 2; neuronNum++){
            Neuron neuronProp = neurons[neuronNum].GetComponent<Neuron>();

            if(neuronProp.connectedNeurons.Count != neuronProp.connections.Count){
                Debug.Log("Number of connected neurons and connections are not equal :( Something went wrong in neuron named: " + neuronProp.gameObject.name);
            }
            if(neuronProp.connectedNeurons.Count != neuronProp.connectionsDir.Count){
                Debug.Log("Number of connected neurons and connection directions are not equal :( Something went wrong in neuron named: " + neuronProp.gameObject.name);
            }
        }
    }

    public void AddToConnectionList(GameObject neuronFrom, GameObject neuronTo){
        allConections[0].Add(neuronFrom);
        allConections[1].Add(neuronTo);

        //Debug.Log(allConections[0].Count);
    }

    public void RemoveFromConnectionList(GameObject neuronFrom, GameObject neuronTo){
        if(!IsConnectionSaved(neuronFrom, neuronTo)){
            Debug.Log( $"You try to remove connection between: {neuronFrom.name} and {neuronTo.name} but it doesn't exists on list" );
        }else{
            for(int i = 0; i < allConections[0].Count; i++){
                if(GameObject.ReferenceEquals( neuronFrom, allConections[0][i])){
                    if(GameObject.ReferenceEquals( neuronTo, allConections[1][i])){
                        allConections[0].RemoveAt(i);
                        allConections[1].RemoveAt(i);
                    }
                }
            }
        }
    }

    public bool IsConnectionSaved(GameObject neuronFrom, GameObject neuronTo){ // Check if connection between neurons is already saved at 'allConections' list
        for(int i = 0; i < allConections[0].Count; i++){
            if(GameObject.ReferenceEquals( neuronFrom, allConections[0][i])){
                if(GameObject.ReferenceEquals( neuronTo, allConections[1][i])){
                    return true;
                }
            }
        }
        return false;
    }

    public GameObject FindConnection(GameObject neuronFrom, GameObject neuronTo){
        GameObject connection = neuronFrom.GetComponent<Neuron>().connections.Find(obj => (GameObject.ReferenceEquals(obj.GetComponent<Connection>().neuronFrom, neuronFrom) && 
                                                                                            GameObject.ReferenceEquals(obj.GetComponent<Connection>().neuronTo, neuronTo)) );

        return connection;            
    }

    public void MergeConnections(Connection connection1, Connection connection2, bool isConnectionInitiated){
        if(isConnectionInitiated){
            connection1.isConnectionMerged = true;
            connection2.isConnectionMerged = true;
        }

        connection1.LineFrom.GetComponent<LineRenderer>().SetPosition(0, connection2.LineFrom.GetComponent<LineRenderer>().GetPosition(1));
        connection2.LineFrom.GetComponent<LineRenderer>().SetPosition(0, connection1.LineFrom.GetComponent<LineRenderer>().GetPosition(1));
    }

    public void SplitConnections(Connection connection1, Connection connection2){
        connection1.isConnectionMerged = false;
        connection2.isConnectionMerged = false;

        connection1.LineFrom.GetComponent<LineRenderer>().SetPosition(0, connection2.LineTo.GetComponent<LineRenderer>().GetPosition(0));
        connection2.LineFrom.GetComponent<LineRenderer>().SetPosition(0, connection1.LineTo.GetComponent<LineRenderer>().GetPosition(0));
    }
}
