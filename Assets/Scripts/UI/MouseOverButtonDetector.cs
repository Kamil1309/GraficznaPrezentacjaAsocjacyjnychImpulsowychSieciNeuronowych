using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class MouseOverButtonDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler// required interface when using the OnPointerEnter method.
{
    [SerializeField] SelectManager selectManager;

    public void OnPointerEnter(PointerEventData eventData){
        selectManager.mouseOverUpdateSynapseButton = true;
    }

    public void OnPointerExit(PointerEventData eventData){
        selectManager.mouseOverUpdateSynapseButton = false;
    }
}

