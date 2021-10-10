 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowNeuronProp : MonoBehaviour
{
    private bool dataShowInitialization = false;
    private bool linesInit = false;
    private bool panelsInit = false;

    private IEnumerator coroutine;

    public GameObject[] lines = new GameObject[4];
    public GameObject[] panels = new GameObject[4];

    private void Start() {

        lines = GetComponent<Neuron>().lines;
        panels = GetComponent<Neuron>().panels;

        foreach( GameObject obj in lines){
            obj.GetComponent<LineProperties>().resetLine();
        }
        foreach( GameObject obj in panels){
            obj.GetComponent<PanelProperties>().resetPanel();
        }

    }

    void OnMouseOver()
    {
        if(!dataShowInitialization)
        {
            //Debug.Log("Mouse is over GameObject.");

            dataShowInitialization = true;
            linesInit = true;
            panelsInit = true;

            coroutine = ShowProperties();
            StartCoroutine(coroutine);
        }
    }

    void OnMouseExit()
    {
        dataShowInitialization = false;

        foreach( GameObject obj in lines){
            obj.GetComponent<LineProperties>().resetLine();
        }
        foreach( GameObject obj in panels){
            obj.GetComponent<PanelProperties>().resetPanel();
        }
        StopCoroutine(coroutine);
    }

    private IEnumerator ShowProperties()
    {
        //Debug.Log(dataShowInitialization);
        while (linesInit || panelsInit)
        {
            if(linesInit)
            {
                foreach( GameObject obj in lines){
                    linesInit = obj.GetComponent<LineProperties>().expandLine();
                }
            }
            if(panelsInit)
            {
                foreach( GameObject obj in panels){
                    panelsInit = obj.GetComponent<PanelProperties>().showPanel();
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
