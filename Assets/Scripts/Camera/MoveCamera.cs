using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float scrollSpeed = 0.4f;
    public float moveSpeedDivider = 40.0f;
    public float rightLeftLimit = 20.0f;
    public float topBottomLimit = 20.0f;

    public float downLimit = 1;
    public float upLimit = 10;

    public bool mouseMidButtDown = false;
    Vector3 lastMousePos;

    SelectManager selectManager;
    NeuronsManager neuronsManager;

    //public bool isCoursorAtNeuron = false;
    private void Awake() {
        selectManager = FindObjectOfType<SelectManager>();
        neuronsManager = FindObjectOfType<NeuronsManager>();
    }
    
    private void Update() {
        if(selectManager.selectedObject == null){
            if(Input.GetAxisRaw("Mouse ScrollWheel") > 0){ // zooming in
                Vector3 newPos = new Vector3(transform.position.x, transform.position.y - scrollSpeed, transform.position.z);

                SetCameraPosition(newPos);
            }
            else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0){ // zoom out
                Vector3 newPos = new Vector3(transform.position.x, transform.position.y + scrollSpeed, transform.position.z);
                SetCameraPosition(newPos);
            }
        }
        
        if (Input.GetMouseButtonDown(2)){
            lastMousePos = Input.mousePosition;
            mouseMidButtDown = true;
        }
        
        if (Input.GetMouseButtonUp(2)){
            mouseMidButtDown = false;
        }

        if(mouseMidButtDown){
            Vector3 newPos = new Vector3(transform.position.x + (Input.mousePosition.x - lastMousePos.x)/moveSpeedDivider, 
                                        transform.position.y, 
                                        transform.position.z + (Input.mousePosition.y - lastMousePos.y)/moveSpeedDivider);

            SetCameraPosition(newPos);
        
            lastMousePos = Input.mousePosition;
        }
    }

    public void SetCameraPosition(Vector3 newPos){
        if(newPos.x < -rightLeftLimit) // right/left bound
            newPos = new Vector3(-rightLeftLimit, newPos.y, newPos.z);  
        else if(newPos.x > rightLeftLimit)
            newPos = new Vector3(rightLeftLimit, newPos.y, newPos.z);  

        if(newPos.z < -topBottomLimit) // top/bottom bound
            newPos = new Vector3(newPos.x, newPos.y, -topBottomLimit);  
        else if(newPos.z > topBottomLimit)
            newPos = new Vector3(newPos.x, newPos.y, topBottomLimit);

        if(newPos.y < downLimit) // up/down bound
            newPos = new Vector3(newPos.x, downLimit, newPos.z);  
        else if(newPos.y > upLimit)
            newPos = new Vector3(newPos.x, upLimit, newPos.z);

        transform.position = newPos;
        neuronsManager.UpdateAllNeuronsTextPosition();
    }
}
