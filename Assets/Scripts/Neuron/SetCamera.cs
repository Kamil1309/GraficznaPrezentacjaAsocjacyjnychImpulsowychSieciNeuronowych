using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCamera : MonoBehaviour
{
    void Start()
    {
        SetMainCameraAsCamera();
    }

    public void SetMainCameraAsCamera(){
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
