using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationTest : MonoBehaviour
{
    public FunctionsToControlTheGraphicalEnvironment simFunctions;
    
    void Start()
    {
        simFunctions = GetComponent<FunctionsToControlTheGraphicalEnvironment>();

        simFunctions.CreateNeuron("Very");
        simFunctions.CreateNeuron("My");
        simFunctions.CreateNeuron("Is");
        simFunctions.CreateNeuron("Small");
        simFunctions.CreateNeuron("Monkey");
        simFunctions.CreateNeuron("A");
        simFunctions.CreateNeuron("Have");
        simFunctions.CreateNeuron("I");

        StartCoroutine(ExecuteAfterTime(1f));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds( 2.0f);

        simFunctions.CreateNeuron("Frog");

        yield return new WaitForSeconds( 2.0f);

        List<string> listOfNeurons = simFunctions.GetListOfNeurons();
        for(int i = 0; i < listOfNeurons.Count; i++){
            Debug.Log(listOfNeurons[i]);
        }

        yield return new WaitForSeconds( 2.0f);

        simFunctions.CreateConnection("Monkey", "Frog");
        simFunctions.CreateConnection("Have", "A");
        simFunctions.CreateConnection(0, 5);
        simFunctions.CreateConnection(5, 0);

        yield return new WaitForSeconds( 2.0f);

        simFunctions.DestroyConnection("Have", "A");
        simFunctions.DestroyConnection(0, 5);

        yield return new WaitForSeconds( 2.0f);

        simFunctions.SetNeuronName("Frog", "Dog");
        simFunctions.SetNeuronName(0, "Bird");

        yield return new WaitForSeconds( 2.0f);

        simFunctions.SetNeuronState("Dog", "Loading");
        simFunctions.SetNeuronState(0, "Activation");

        yield return new WaitForSeconds( 2.0f);

        simFunctions.SetNeuronParameters("Dog", 0.1f, 0.8f, 0.7f, 0.0f);

        yield return new WaitForSeconds( 2.0f);

        simFunctions.SetConnectionValue("Monkey", "Dog", 95);

        yield return new WaitForSeconds( 2.0f);

        simFunctions.SendSignal("Monkey", "Dog");
        simFunctions.SendSignal(5, 0);

        yield return new WaitForSeconds( 2.0f);

        simFunctions.DestroyNeuron(0);

    }
}
