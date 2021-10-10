using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePlaceFinder : MonoBehaviour
{
    public float maxDistBetweenNeurons = 4.0f; // Neurons will not appear in shorter distance from other neurons then this
    public float distBetweenNeurons = 4.5f; // Neuron new places will be searched in that distance at circle
    public float changeOfRadiusRange = 4.0f; // Change of circle radius while searching new place 

    public Vector3 FindFreePlace(){
        Vector3 freePlace = Vector3.zero;
        int radiusChanges = 0;
        float angle = 0.0f;
        float angleShift = 0.0f;

        while(CheckIfWithinNeuronsRange(freePlace)){
            if(radiusChanges != 0 &  2*Mathf.PI  > angle){
                freePlace = new Vector3( radiusChanges * changeOfRadiusRange * Mathf.Cos(angle), 0.0f, radiusChanges * changeOfRadiusRange * Mathf.Sin(angle));
                angle += angleShift;
            }else{
                radiusChanges += 1;
                angleShift = Mathf.Acos(1 - distBetweenNeurons * distBetweenNeurons/(2*(radiusChanges * changeOfRadiusRange)*(radiusChanges * changeOfRadiusRange)));

                angle = 0.0f;
            }
        }
        return freePlace;
    }

    private bool CheckIfWithinNeuronsRange(Vector3 pointToCheck){
        List<Transform> allNeurons = transform.GetAllChildren();
        pointToCheck = new Vector3(pointToCheck.x, 0.0f, pointToCheck.z);

        for(int i = 0; i < allNeurons.Count; i++){
            Vector3 neuronPos = new Vector3(allNeurons[i].transform.position.x, 0.0f, allNeurons[i].transform.position.z);
            if(Vector3.Magnitude(neuronPos - pointToCheck) < maxDistBetweenNeurons){
                return true;
            }
        }
        return false;
    }
}
