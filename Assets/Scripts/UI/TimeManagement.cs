using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeManagement : MonoBehaviour
{
    private bool isSimulationPaused = false;
    private int actualSimulationSpeedIndex;

    private float [] speedPossibleLevels = new float[3]  { 0.5f, 1.0f, 2.0f}; // Possible speed levels from lowest to highest

    public GameObject pausePanel;

    public Sprite pauseSprite;
    public Sprite playSprite;

    public GameObject pauseResumeButton;

    public Text actualSpeedText; 

    public GameObject neurons;
    public GameObject connections;

    private void Start() {
        actualSimulationSpeedIndex = Array.IndexOf(speedPossibleLevels, 1.0f);
    }

    public void ChangeSimulationState(){
        isSimulationPaused = !isSimulationPaused;

        if(isSimulationPaused){
            PauseSimulation();
            Debug.Log("SimulationPaused");
        }else{
            ResumeSimulation();
            Debug.Log("SimulationResumed");
        }
    }

    void PauseSimulation ()
    {
        pausePanel.SetActive(true);
        pauseResumeButton.GetComponent<Image>().sprite = playSprite;
        Time.timeScale = 0;
    }

    void ResumeSimulation ()
    {
        pausePanel.SetActive(false);
        pauseResumeButton.GetComponent<Image>().sprite = pauseSprite;
        Time.timeScale = 1;
    }

    public void SpeedUpTheSimulation(){
        if(actualSimulationSpeedIndex < speedPossibleLevels.Length - 1){
            actualSimulationSpeedIndex += 1;

            actualSpeedText.text = "x" + speedPossibleLevels[actualSimulationSpeedIndex].ToString();

            SetAllNeuronsSimulationSpeed();
            SetAllConnectionsSimulationSpeed();

            Debug.Log("Simulation speeded up, actual speed:  " + speedPossibleLevels[actualSimulationSpeedIndex]);
        }
    }

    public void SlowDownTheSimulation(){ 
        if(actualSimulationSpeedIndex > 0){
            actualSimulationSpeedIndex -= 1;

            actualSpeedText.text = "x" + speedPossibleLevels[actualSimulationSpeedIndex].ToString();

            SetAllNeuronsSimulationSpeed();
            SetAllConnectionsSimulationSpeed();

            Debug.Log("Simulation slowed down, actual speed:  " + speedPossibleLevels[actualSimulationSpeedIndex]);
        }
    }

    public void SetAllNeuronsSimulationSpeed(){
        
        List<Transform> allNeurons = neurons.transform.GetAllChildren();
        for(int i = 0; i <= allNeurons.Count - 1; i++){
            allNeurons[i].GetComponent<NeuronsRepulsion>().actualSimulationSpeed = speedPossibleLevels[actualSimulationSpeedIndex];
        }
    }

    public void SetAllConnectionsSimulationSpeed(){
        
        List<Transform> allConnections = connections.transform.GetAllChildren();

        for(int i = 0; i <= allConnections.Count - 1; i++){
            allConnections[i].GetComponent<ConnectionSignal>().actualSimulationSpeed = speedPossibleLevels[actualSimulationSpeedIndex];
            allConnections[i].GetComponent<ConnectionStretching>().actualSimulationSpeed = speedPossibleLevels[actualSimulationSpeedIndex];
        }
    }

    public float GetActualSimulationSpeed(){
        return speedPossibleLevels[actualSimulationSpeedIndex];
    }
}
