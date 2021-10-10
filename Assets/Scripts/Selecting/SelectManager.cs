using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectManager : MonoBehaviour
{
    public GameObject selectedObject;
    public GameObject hoveredObject;

    [SerializeField] GameObject selectPanel;
    [SerializeField] Button updateSynapseImgSizeButton;
    [SerializeField] Text textTitle;
    [SerializeField] Text textDesctipition;

    [HideInInspector] public bool mouseOverUpdateSynapseButton = false;

    private void Update() {
        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

        RaycastHit hitInfo;

        if( Physics.Raycast( ray, out hitInfo ) ){
            
            GameObject hitObject = hitInfo.transform.gameObject;

            HoverObject(hitObject);
        }else{
            ClearHover();
        }

        if(Input.GetMouseButtonDown(0)){
            SelectObject(hoveredObject);
        }

        if(selectedObject != null){
            if(Input.GetAxisRaw("Mouse ScrollWheel") < 0){// zooming in 
                selectedObject.GetComponent<Selectable>().OnMouseWheelMovingMinus();
            }else if(Input.GetAxisRaw("Mouse ScrollWheel") > 0){
                selectedObject.GetComponent<Selectable>().OnMouseWheelMovingPlus();
            }
        }
    }

    void HoverObject(GameObject obj){
        if(hoveredObject != null){
            if(hoveredObject.GetComponent<Selectable>() != null){
                if(GameObject.ReferenceEquals( obj, hoveredObject)){
                    return;
                }

                ClearHover();
            }
        }
        
        hoveredObject = obj;
        ShowSelectPanel(hoveredObject);
    }

    void ClearHover(){
        hoveredObject = null;

        if(selectedObject == null){
            HideSelectPanel();
        }else{
            ShowSelectPanel(selectedObject);
        }
    }

    void SelectObject(GameObject obj){
        if(obj != null){
            Selectable selectable = obj.GetComponent<Selectable>();
            if(selectable.CanBeSelected()){
                selectedObject = obj;
                ShowSelectPanel(selectedObject);

                selectedObject.GetComponent<Selectable>().Select();
            }
        }else{
            ClearSelection();
        }
    }

    void ClearSelection(){
        if(CheckIfSelectionCanBeCleared()){
            selectedObject = null;
            HideSelectPanel();
        }
    }

    bool CheckIfSelectionCanBeCleared(){
        if(mouseOverUpdateSynapseButton){
            return false;
        }else{
            return true;
        }
    }

    void ShowSelectPanel(GameObject selectedObject){
        selectPanel.SetActive(true);

        if(selectedObject.tag == "Connection"){
            updateSynapseImgSizeButton.gameObject.SetActive(true);
        }else{
            updateSynapseImgSizeButton.gameObject.SetActive(false);
        }

        textTitle.text = selectedObject.GetComponent<Selectable>().CreateTitleText();
        textDesctipition.text = selectedObject.GetComponent<Selectable>().CreateDescriptionText();
    }

    void HideSelectPanel(){
        selectPanel.SetActive(false);
    }
}
