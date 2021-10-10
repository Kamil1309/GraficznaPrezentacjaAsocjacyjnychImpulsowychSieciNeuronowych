using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySynapsesSize : MonoBehaviour
{
    [SerializeField] GameObject selectManager; 

    float defaultSize = 0.8f;

    public void ApplySynapsesSizeFun(){
        defaultSize = selectManager.GetComponent<SelectManager>().selectedObject.GetComponent<Connection>().synapseImageSize;

        List<Transform> allConnections = transform.GetAllChildren();
        
        for(int connectionNum = 0; connectionNum < allConnections.Count; connectionNum++){
            allConnections[connectionNum].gameObject.GetComponent<Connection>().SetSynapsesSize(defaultSize);
        }
    }
}
