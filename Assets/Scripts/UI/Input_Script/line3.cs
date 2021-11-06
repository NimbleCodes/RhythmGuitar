using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line3 : MonoBehaviour
{
    [SerializeField] GameObject Input_line3;
    private void OnMouseEnter()
    {
        if(line1.instance.swipping == true){
            // Debug.Log("line3");
            line1.instance.lineCount ++;
        }
    }
}
