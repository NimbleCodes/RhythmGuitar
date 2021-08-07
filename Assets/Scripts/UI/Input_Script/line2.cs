using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class line2 : MonoBehaviour
{
    [SerializeField] GameObject Input_line2;
    private void OnMouseEnter()
    {
        //if(line1.instance.swipping == true){
            PlayAnimation.instance.Stroke2();
        //}
    }
}
