using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    public bool colorIsWhite = true;

    public void ChangeColorToWhite(){
        Camera.main.backgroundColor = new Color(1f, 1f, 1f, 1f);
        colorIsWhite = true;

        FindObjectOfType<NeuronsManager>().SwitchPropertiesColors(colorIsWhite);
        FindObjectOfType<ChangeConnectionsColor>().SwitchDendritesColors(colorIsWhite);
    }

    public void ChangeColorToBlack(){
        Camera.main.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 1f);
        colorIsWhite = false;

        FindObjectOfType<NeuronsManager>().SwitchPropertiesColors(colorIsWhite);
        FindObjectOfType<ChangeConnectionsColor>().SwitchDendritesColors(colorIsWhite);
    }
}
