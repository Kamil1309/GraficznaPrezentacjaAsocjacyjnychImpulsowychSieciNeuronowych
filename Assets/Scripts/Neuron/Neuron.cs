using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Neuron : Selectable
{
    //public Color myColor;

    [HideInInspector]
    public float radius;

    public float param01;
    public float param02;
    public float param03;
    public float param04;

    [HideInInspector]
    public float[] neuronParams = new float[4];

    #region lines and panels
    [Header("Lines")]
    public GameObject lineTop;
    public GameObject lineBot;
    public GameObject lineLeft;
    public GameObject lineRight;

    [HideInInspector] 
    public GameObject[] lines = new GameObject[4];

    [Header("Panels")]
    public GameObject panelTop;
    public GameObject panelBot;
    public GameObject panelLeft;
    public GameObject panelRight;

    [HideInInspector] public 
    GameObject[] panels = new GameObject[4];
    #endregion

    [Header("Shaders")]
    public Material ActivationShader;
    public Material LoadingShader;
    public Material RefractionShader;

    [Header("Neuron Text")]
    public Text neuronText;

    public List<GameObject> connectedNeurons = new List<GameObject>();
    public List<GameObject> connections;
    public List<string> connectionsDir;

    public bool connectionInProgress = false;
    public bool signalInMove = false;

    MoveNeuron moveNeuron;

    private void Awake() {
        radius = 0.5f;

        lines[0] = lineTop;
        lines[1] = lineRight;
        lines[2] = lineBot;
        lines[3] = lineLeft;

        panels[0] = panelTop;
        panels[1] = panelRight;
        panels[2] = panelBot;
        panels[3] = panelLeft;

        moveNeuron = GetComponent<MoveNeuron>();
    }

    private void Start() {
        SetParameters(param01, param02, param03, param04);

        GetComponent<NeuronTextManager>().UpdateTextPositionTowardsCamera();
        GetComponent<NeuronTextManager>().SetText(neuronText.text);
    }

    public void SetNeuronName(string newName){
        gameObject.name = newName;
        neuronText.text = newName;
    }

    public override bool CanBeSelected()
    {
        return true;
    }

    public override void Select()
    {
        //Debug.Log("You have selected " + gameObject.name);
    }

    public override void OnMouseWheelMovingPlus(){
        if(!connectionInProgress){
            moveNeuron.upperTheNeuron();
        }
    }

    public override void OnMouseWheelMovingMinus(){
        if(!connectionInProgress){
            moveNeuron.lowerTheNeuron();
        }
    }

    public override string CreateTitleText(){
        string title = "Neuron '" + neuronText.text + "'";

        return title;
    }

    public void SetParameters(float TotRelaxRate = 0, float AfterActivationRelaxRate = 0, float ActivTreshold = 0, float ExcitedState = 0){
        if(TotRelaxRate >= 0 & TotRelaxRate < 1){
            neuronParams[0] = TotRelaxRate;
        }else{
            Debug.Log("Wrong 'TotRelaxRate' param value");
        }
        if(AfterActivationRelaxRate >= 0.5f & AfterActivationRelaxRate < 1){
            neuronParams[1] = AfterActivationRelaxRate;
        }else{
            Debug.Log("Wrong 'AfterActivationRelaxRate' param value");
        }
        if(ActivTreshold >= 0){
            neuronParams[2] = ActivTreshold;
        }else{
            Debug.Log("Wrong 'ActivTreshold' param value");
        }
        if(ExcitedState >= 0){
            neuronParams[3] = ExcitedState;
        }else{
            Debug.Log("Wrong 'ExcitedState' param value");
        }
        GetComponent<SetPanelsText>().SetAllPanelsTexts();
    }

    public override string CreateDescriptionText(){
        string description = "Total relaxation rate : " + neuronParams[0].ToString() + "\n"
                            +"After activation relaxation rate: " + neuronParams[1].ToString() + "\n"
                            +"Activation treshold : " + neuronParams[2].ToString() + "\n"
                            +"Excited state : " + neuronParams[3].ToString() + "\n";

        return description;
    }

    public void UpdateConnectionInProgress(){
        for(int connectionNum = 0; connectionNum < connections.Count; connectionNum++){
            if(connections[connectionNum].GetComponent<ConnectionStretching>().connectionInProgress){
                connectionInProgress = true;
                return;
            }
        }
        connectionInProgress = false;
    }

    public void UpdateSignalInMove(){
        for(int connectionNum = 0; connectionNum < connections.Count; connectionNum++){
            if(connections[connectionNum].GetComponent<ConnectionSignal>().signalInMoveFull){
                signalInMove = true;
                return;
            }
        }
        signalInMove = false;
    }

    public bool IsConnectedWith(GameObject neuronToCheck){
        if(connectedNeurons.Contains(neuronToCheck)){
            return true;
        }else{
            return false;
        }
    }

    public int IsConnectedWith2(GameObject neuronToCheck){
        int howMany = 0;

        for(int i = 0; i < connectedNeurons.Count; i++){
            if(ReferenceEquals(neuronToCheck, connectedNeurons[i])){
                howMany++;
            }
        }

        return howMany;
    }

    public Tuple<int?, int?> FindConnectedNeuronIndex(GameObject neuronToFind){
        int? index1 = null;
        int? index2 = null;

        for(int i = 0; i < connectedNeurons.Count; i++){
            if(GameObject.ReferenceEquals(neuronToFind, connectedNeurons[i])){
                if(index1 == null){
                    index1 = i;
                }else{
                    index2 = i;
                }
            }
        }

        var indexes = Tuple.Create<int?, int?>(index1, index2);

        return indexes;
    }

    public int AddConnectedNeuron(GameObject neuron){
        connectedNeurons.Add(neuron);
        
        return connectedNeurons.Count - 1;
    }

    // public void DeleteConnectedNeuron(GameObject neuron){
    //     if(!connectedNeurons.Remove(neuron)){
    //         Debug.Log("You tried to remove neuron: " + neuron.name + " from connected neurons list on: " + gameObject.name + " but it wasn't there");
    //     }
    // }

    public void DeleteConnectedNeuron(int index){
        if(connectedNeurons.Count > index){
            connectedNeurons.RemoveAt(index);
        }else{
            Debug.LogError("You tried to remove neuron with index: " + index + " but index exceeds the radius of the list");
        }
    }

    public void AddConnection(int indexNum, GameObject connection){
        if(connections.Count - 1 >= indexNum){ // Means that connection list already got good radius
            if(connections[indexNum] != null){
                Debug.LogError("You try to set connection but it's already set");
            }else{
                connections[indexNum] = connection;
            }
        }else if(connections.Count - 1 < indexNum){
            for(int i = 0; i < indexNum - (connections.Count - 1); i++){
                connections.Add(null);
            }
            connections[indexNum] = connection;
        }
    }

    public int AddConnection(GameObject connection){
        connections.Add(connection);

        return connections.Count - 1;
    }

    // public void DeleteConnectionFromList(GameObject connection){
    //     if(!connections.Remove(connection)){
    //         Debug.Log("You tried to remove connection: " + connection.name + " from connections list on: " + gameObject.name + " but it wasn't there");
    //     }
    // }

    public void DeleteConnectionFromList(int index){
        if(connections.Count > index){
            connections.RemoveAt(index);
        }else{
            Debug.Log("You tried to remove connection with index: " + index + " from connections list on: " + gameObject.name + " but under that index there is nothing");
        }
    }

    public void AddConnectionDir(int indexNum, string direction){
        if(connectionsDir.Count - 1 >= indexNum){ // Means that direction list already got good radius
            if(connectionsDir[indexNum] == "from" || connectionsDir[indexNum] == "to"){
                Debug.LogError("You try to set direction but it's already set");
            }else{
                connectionsDir[indexNum] = direction;
            }
        }else if(connectionsDir.Count - 1 < indexNum){
            for(int i = 0; i < indexNum - (connectionsDir.Count - 1); i++){
                connectionsDir.Add(null);
            }
            connectionsDir[indexNum] = direction;
        }
    }

    public int AddConnectionDir(string direction){
        connectionsDir.Add(direction);

        return connectionsDir.Count - 1;
    }

    public void DeleteConnectionDir(int index){
        if(connectionsDir.Count > index){
            connectionsDir.RemoveAt(index);
        }else{
            Debug.Log("You tried to remove direction with index: " + index + " from directions list on: " + gameObject.name + " but under that index there is nothing");
        }
    }

    public int? FindConnectionIndex(GameObject connection){
        for(int connNum = 0; connNum < connections.Count; connNum++){
            if(GameObject.ReferenceEquals(connection, connections[connNum])){
                return connNum;
            }
        }

        return null;
    }

    public void SetActivationState(){
        GetComponent<Renderer>().material = ActivationShader;
    }

    public void SetLoadingState(){
        GetComponent<Renderer>().material = LoadingShader;
    }

    public void SetRefractionState(){
        GetComponent<Renderer>().material = RefractionShader;
    }

    public void SendSignal(GameObject neuronToSend){
        if(connectedNeurons.Contains(neuronToSend)){
            var indexes = FindConnectedNeuronIndex(neuronToSend);
            int index = 0;
            bool found = false;

            if(connectionsDir[(int)indexes.Item1] == "from"){
                index = (int)indexes.Item1;
                found = true;
            }else if(indexes.Item2 != null){
                if(connectionsDir[(int)indexes.Item2] == "from"){
                    index = (int)indexes.Item2;
                    found = true;
                }
            }

            if(found){
                GameObject connection = connections[index];
                
                if(!connection.GetComponent<ConnectionSignal>().signalInMoveFull){
                    connection.GetComponent<ConnectionSignal>().SendSignalForward();
                }else{
                    Debug.Log("Signal not sended, you try to send signal while another is shipped");
                }
            }else{
                Debug.Log("You try to send signal to neuron, its connected but not in right way");
            }

        }else{
            Debug.Log("You try to send signal to neuron, but its not connected");
        }
    }

}
