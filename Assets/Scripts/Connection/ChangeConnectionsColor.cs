using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeConnectionsColor : MonoBehaviour
{
    public bool colorIsWhite = true;

    public void SwitchDendritesColors(bool colorIsWhiteOrg){
        if(colorIsWhite != colorIsWhiteOrg){
            colorIsWhite = colorIsWhiteOrg;

            Color newColor;
            if(colorIsWhite){
                newColor = new Color(0.0f, 0.0f, 0.0f, 1f);
            }
            else{
                newColor = new Color(1f, 1f, 1f, 1f);
            }
                
            List<Transform> allConnections = gameObject.transform.GetAllChildren();
            foreach(Transform connection in allConnections){
                connection.gameObject.GetComponent<Connection>().SetDendritesColor(newColor);
            }
        }
    }
}
