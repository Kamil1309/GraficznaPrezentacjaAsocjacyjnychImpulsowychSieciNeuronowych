using System.Collections.Generic;
using UnityEngine;

public static class TransformHelper
 {
    public static List<Transform> GetAllChildren(this Transform aTransform, List<Transform> aList = null, bool recursive = false)
    {
        if (aList == null)
            aList = new List<Transform>();
        int start = aList.Count;
        for (int n = 0; n < aTransform.childCount; n++)
            aList.Add(aTransform.GetChild(n));
        if(recursive){
            for (int i = start; i < aList.Count; i++)
            {
                var t = aList[i];
                for (int n = 0; n < t.childCount; n++)
                    aList.Add(t.GetChild(n));
            }
        }
        return aList;
    }
 }