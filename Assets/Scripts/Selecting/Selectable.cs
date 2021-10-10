using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public virtual bool CanBeSelected(){
        return false;
    }

    public virtual void Select(){}

    public virtual void OnMouseWheelMovingPlus(){}
    public virtual void OnMouseWheelMovingMinus(){}

    public virtual string CreateTitleText(){
        return "";
    }
    public virtual string CreateDescriptionText(){
        return "";
    }
}
