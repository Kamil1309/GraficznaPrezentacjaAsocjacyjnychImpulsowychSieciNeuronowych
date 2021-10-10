using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelProperties : MonoBehaviour
{
    public float showUpSpeed = 1/255f;

    public Text panelsText;

    public void resetPanel(){
        if(FindObjectOfType<ChangeColor>().colorIsWhite)
            gameObject.GetComponent<Image>().color = new Color(1/4f, 1/4f, 1/4f, 0/255f);
        else
            gameObject.GetComponent<Image>().color = new Color(3/4f, 3/4f, 3/4f, 0/255f);
        
        panelsText.GetComponent<Text>().color = new Color(1, 1, 1, 0/255f);
    }

    public bool showPanel(){

        if(FindObjectOfType<ChangeColor>().colorIsWhite)
            gameObject.GetComponent<Image>().color = new Color(1/4f, 1/4f, 1/4f, gameObject.GetComponent<Image>().color.a + showUpSpeed);
        else
            gameObject.GetComponent<Image>().color = new Color(3/4f, 3/4f, 3/4f, gameObject.GetComponent<Image>().color.a + showUpSpeed);

        panelsText.GetComponent<Text>().color = new Color(1, 1, 1, panelsText.GetComponent<Text>().color.a + 2*showUpSpeed);
        
        if(gameObject.GetComponent<Image>().color.a >= 160/255f){
            return false;
        }
        else{
            return true;
        }
    }
}
