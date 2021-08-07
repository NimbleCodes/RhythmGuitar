using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KusoGame.Signals;

public class line4 : MonoBehaviour
{
    [SerializeField] GameObject Input_line4;
    public Switch inputSwitch;

    private void OnMouseEnter()
    {
        // if(line1.instance.swipping == true){
            
        // }
        inputSwitch.Invoke(4);
    }
}
