using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LineProperties : MonoBehaviour
{
    public float speed = 0.001f;

    public Vector3 resetPos = new Vector3(0.0f, 0.0f, 0.0f);

    private  Vector3[] endPos = new Vector3[2];
    public Vector3 endPos0 = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 endPos1 = new Vector3(0.0f, 0.0f, 0.0f);

    public LineRenderer lineRenderer;
    
    private void Awake() {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    private void Start() {
        endPos[0] = endPos0;
        endPos[1] = endPos1;
    }

    public void resetLine(){
        lineRenderer.SetPosition(0, resetPos);
        lineRenderer.SetPosition(1, resetPos);
    }

    public bool expandLine(){
        for(int i = 0; i < 2; i++){
            if(Math.Abs(lineRenderer.GetPosition(0).x) > 0.5f || Math.Abs(lineRenderer.GetPosition(0).y) > 0.5f || Math.Abs(lineRenderer.GetPosition(0).z) > 0.5f)
                lineRenderer.SetPosition(i, new Vector3(lineRenderer.GetPosition(i).x + GetSign(endPos[i].x - resetPos.x)*speed,
                                                        lineRenderer.GetPosition(i).y + GetSign(endPos[i].y - resetPos.y)*speed, 
                                                        lineRenderer.GetPosition(i).z + GetSign(endPos[i].z - resetPos.z)*speed));
            else
            {
                return false;
            }
        }
        return true;
    }

    public float GetSign(float operation){
        if(operation > 0)
            return 1;
        else
            if(operation < 0)
                return -1;
            else
                return 0;
    }

    
}
