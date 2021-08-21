using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using KusoGame.Signals;

public class line2 : MonoBehaviour
{
    [SerializeField] GameObject Input_line2;
    public Switch inputSwitch;
    private void OnMouseEnter()
    {
        //if(line1.instance.swipping == true){
            inputSwitch.Invoke(2);
            PlayAnimation.instance.Stroke2();
        //}
    }
}
